using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using System;


public class InkManager : MonoBehaviour
{
    private const string WeirdFactorVariableName = "weirdFactor";
    private const string HasCafeBottleVariableName = "hasCafeBottle";
    private const string HasPsychicBottleVariableName = "hasPsychicBottle";
    private const string HasReceptionBottleVariableName = "hasReceptionBottle";

    [SerializeField] private TextAsset inkJsonAsset;
    [SerializeField] private string startKnotName = "Start";
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject narratorPanel;
    private Story story;
    [SerializeField] private GameObject dialoguePrefabPlayer, dialoguePrefabOther;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private GameObject playerCharacterPanel;
    // [SerializeField] private GameObject continueButton;
    [SerializeField] private CharacterSpriteHolder characterSpriteHolder;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private CardGame cardGame;
    [SerializeField] private Image background;
    [SerializeField] private Sprite recptionBackground, psychicBackground, cafeBackground;
    [SerializeField] private Sprite weddingBackground, islandBackground;
    [SerializeField] private Texture2D[] receptionBackgroundOverlays;
    [SerializeField] private Texture2D[] psychicBackgroundOverlays;
    [SerializeField] private Texture2D[] cafeBackgroundOverlays;
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private RorschachTest rorschachTest;
    [SerializeField] private CanvasGroup startScreen;
    private TalkingBounceAnimator playerTalkingBounceAnimator;
    private CanvasGroup playerCharacterCanvasGroup;
    private Vector3 playerCharacterVisiblePosition;
    private bool isSceneTransitioning;
    private bool isTyping;
    private bool skipTypewriter;
    private TextMeshProUGUI activeDialogueText;
    private RectTransform activeDialogueRect;
    private string activeDialogueLine;
    private GameObject dialogueToReplace;
    private character currentCharacter;
    private int currentWeirdFactor;
    private bool hasCafeBottle;
    private bool hasPsychicBottle;
    private bool hasReceptionBottle;
    private GameObject backgroundOverlay;
    private Sprite backgroundOverlaySprite;

    void Start()
    {
        if (cardGame == null)
        {
            cardGame = FindFirstObjectByType<CardGame>();
        }

        if (rorschachTest == null)
        {
            rorschachTest = FindFirstObjectByType<RorschachTest>();
        }

        playerTalkingBounceAnimator = GetOrAddTalkingBounceAnimator(playerCharacterPanel);
        SetupPlayerCharacterPanel();
        // StartStory();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }

    public void StartGame()
    {
        musicManager.EnsureBackgroundAudioPlaying();
        musicManager.PlayClickSFX();
        startScreen.DOFade(0,0.3f);
        startScreen.interactable= false;
        startScreen.blocksRaycasts=false;
        StartStory();
    }

    private void StartStory()
    {
        cardGame?.ResetCardSequence();
        ResetInkStory();

        ClearDialogue();
        ClearChoices();
        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        narratorPanel.transform.DOLocalMoveY(-840, 0f);
        choicePanel.transform.DOLocalMoveY(-840, 0f);
        DisplayNextLine();
    }

    private void ResetInkStory()
    {
        if (story != null)
        {
            story.RemoveVariableObserver(OnWeirdFactorChanged, WeirdFactorVariableName);
            RemoveBottleVariableObservers();
        }

        story = new Story(inkJsonAsset.text);
        if (string.IsNullOrWhiteSpace(startKnotName))
        {
            Debug.LogError("Cannot reset the Ink story: no start knot name is assigned.");
        }
        else
        {
            try
            {
                story.ChoosePathString(startKnotName.Trim());
            }
            catch (Exception exception)
            {
                Debug.LogError($"Cannot reset the Ink story to knot '{startKnotName}': {exception.Message}");
            }
        }
        currentWeirdFactor = Convert.ToInt32(story.variablesState[WeirdFactorVariableName]);
        hasCafeBottle = Convert.ToBoolean(story.variablesState[HasCafeBottleVariableName]);
        hasPsychicBottle = Convert.ToBoolean(story.variablesState[HasPsychicBottleVariableName]);
        hasReceptionBottle = Convert.ToBoolean(story.variablesState[HasReceptionBottleVariableName]);
        story.ObserveVariable(WeirdFactorVariableName, OnWeirdFactorChanged);
        story.ObserveVariable(HasCafeBottleVariableName, OnBottleVariableChanged);
        story.ObserveVariable(HasPsychicBottleVariableName, OnBottleVariableChanged);
        story.ObserveVariable(HasReceptionBottleVariableName, OnBottleVariableChanged);
        musicManager?.SetWeirdFactor(currentWeirdFactor);
    }

    private void OnDestroy()
    {
        if (story != null)
        {
            story.RemoveVariableObserver(OnWeirdFactorChanged, WeirdFactorVariableName);
            RemoveBottleVariableObservers();
        }
    }

    private void OnWeirdFactorChanged(string variableName, object newValue)
    {
        currentWeirdFactor = Convert.ToInt32(newValue);

        if (musicManager == null)
        {
            Debug.LogWarning("Cannot update the music weird factor: no MusicManager is assigned.");
            return;
        }

        musicManager.SetWeirdFactor(currentWeirdFactor);
    }

    private void RemoveBottleVariableObservers()
    {
        story.RemoveVariableObserver(OnBottleVariableChanged, HasCafeBottleVariableName);
        story.RemoveVariableObserver(OnBottleVariableChanged, HasPsychicBottleVariableName);
        story.RemoveVariableObserver(OnBottleVariableChanged, HasReceptionBottleVariableName);
    }

    private void OnBottleVariableChanged(string variableName, object newValue)
    {
        bool hasBottle = Convert.ToBoolean(newValue);
        bool previouslyHadBottle;

        switch (variableName)
        {
            case HasCafeBottleVariableName:
                previouslyHadBottle = hasCafeBottle;
                hasCafeBottle = hasBottle;
                break;
            case HasPsychicBottleVariableName:
                previouslyHadBottle = hasPsychicBottle;
                hasPsychicBottle = hasBottle;
                break;
            case HasReceptionBottleVariableName:
                previouslyHadBottle = hasReceptionBottle;
                hasReceptionBottle = hasBottle;
                break;
            default:
                return;
        }

        if (hasBottle && !previouslyHadBottle)
        {
            musicManager?.PlayBottlePickUpSFX();
        }
    }

    public void DisplayNextLine()
    {
        if (story == null)
        {
            return;
        }

        if (isSceneTransitioning)
        {
            return;
        }

        if (isTyping)
        {
            CompleteActiveDialogueLine();
            return;
        }

        if (story.canContinue)
        {
            narratorPanel.transform.DOLocalMoveY(-840, 0.5f).SetEase(Ease.OutBack);
            string text = story.Continue();
            text = text?.Trim();
            bool advanceAfterCardChoice = false;

            if (story.currentTags.Exists(tag =>
                string.Equals(tag.Trim(), "clearDialogue", StringComparison.OrdinalIgnoreCase)))
            {
                ClearDialogue();
            }

            // character entrance

            foreach (var tag in story.currentTags)
            {
                string[] parts = tag.Split(':');

                if (parts[0] == "entrance")
                {
                    musicManager.PlayCharacterMoveInSFX();
                    if (parts[1] == "deer")
                    {
                        characterSpriteHolder.ShowCharacter(character.doe);
                        currentCharacter = character.doe;
                        return;
                    }
                    else if (parts[1] == "owl")
                    {
                        characterSpriteHolder.ShowCharacter(character.owl);
                        currentCharacter = character.owl;
                        return;
                    }
                    else if (parts[1] == "toad" || parts[1] == "toad1")
                    {
                        characterSpriteHolder.ShowCharacter(character.toad1);
                        currentCharacter = character.toad1;
                        return;
                    }
                    else if (parts[1] == "toad2")
                    {
                        characterSpriteHolder.ShowCharacter(character.toad2);
                        currentCharacter = character.toad2;
                        return;
                    }
                    else if (parts[1] == "toad3")
                    {
                        characterSpriteHolder.ShowCharacter(character.toad3);
                        currentCharacter = character.toad3;
                        return;
                    }
                    else if (parts[1] == "player")
                    {
                        ShowPlayerCharacter();
                        return;
                    }
                    
                }
                else if (parts[0] == "exit" && parts.Length > 1 && parts[1] == "other")
                {
                    characterSpriteHolder.StartCoroutine(characterSpriteHolder.HideCharacter(false));
                    musicManager.PlayCharacterMoveOutSFX();
                    currentCharacter = character.none;
                }
                else if (parts[0] == "exp")
                {
                    // print($"change {currentCharacter}'s expression to {parts[1]}");
                    try
                    {
                        characterSpriteHolder.StartCoroutine(characterSpriteHolder.SetExpression((expression)Enum.Parse(typeof(expression), parts[1], true)));
                    }
                    catch (ArgumentException e)
                    {
                        Debug.LogError($"Invalid expression '{parts[1]}' for character '{currentCharacter}'. Please check the Ink script and ensure the expression is defined correctly. Error: {e.Message}");
                    }
                }
                else if (parts[0] == "scene" && parts.Length > 1)
                {
                    isSceneTransitioning = true;
                    StartCoroutine(SceneTransition(parts[1]));
                    return;
                }
                else if (parts[0] == "cards" && parts.Length > 1)
                {
                    bool isChoiceCardTag = parts[1] == "hoverLeft"
                        || parts[1] == "hoverMiddle"
                        || parts[1] == "hoverRight"
                        || parts[1] == "selectLeft"
                        || parts[1] == "selectMiddle"
                        || parts[1] == "selectRight";

                    if (cardGame != null)
                    {
                        switch (parts[1])
                        {
                            case "fan":
                                cardGame.GenerateDeck();
                                break;
                            case "selectLeft":
                                cardGame.SelectLeftCard();
                                advanceAfterCardChoice = true;
                                break;
                            case "selectMiddle":
                                cardGame.SelectMiddleCard();
                                advanceAfterCardChoice = true;
                                break;
                            case "selectRight":
                                cardGame.SelectRightCard();
                                advanceAfterCardChoice = true;
                                break;
                            case "hoverLeft":
                            case "hoverMiddle":
                            case "hoverRight":
                                // Hover tags configure the choice buttons. Once the
                                // choice is emitted, its text is UI rather than dialogue.
                                advanceAfterCardChoice = true;
                                break;
                            case "discard":
                                cardGame.DiscardSelectedCard();
                                break;
                            case "collapse":
                                cardGame.CollapseDeck();
                                break;
                            default:
                                Debug.LogWarning($"Unknown cards tag action: '{parts[1]}'.");
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Cannot process '{tag}': no CardGame was found in the scene.");
                    }

                    if (!isChoiceCardTag)
                    {
                        return;
                    }
                }
                else if (string.Equals(parts[0].Trim(), "test", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                {
                    string testAction = parts[1].Trim();

                    if (rorschachTest == null)
                    {
                        Debug.LogError($"Cannot process '{tag}': no RorschachTest was found in the scene.");
                        continue;
                    }

                    if (string.Equals(testAction, "complete", StringComparison.OrdinalIgnoreCase))
                    {
                        rorschachTest.Hide();
                    }
                    else if (int.TryParse(testAction, out int testNumber) && testNumber >= 1 && testNumber <= 3)
                    {
                        rorschachTest.ShowTest(testNumber);
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown test tag action: '{testAction}'.");
                    }
                }
            }

            if (advanceAfterCardChoice)
            {
                DisplayNextLine();
                return;
            }

            // dialogue speech bubble
            if (text == null || text == "")
            {
                // print("No text to display.");
                return;
            }
            GameObject prefab;

            bool player = false;
            bool replaceDialogue = HasCurrentTag("replace");
            bool storeDialogueForReplacement = HasCurrentTag("destroy");

            if (HasCurrentTag("player"))
            {
                prefab = dialoguePrefabPlayer;
                player = true;
            }
            else if (HasCurrentTag("narrator"))
            {
                StartCoroutine(DisplayNarratorText(text));
                return;
            }
            else
            {
                prefab = dialoguePrefabOther;
            }

            if (replaceDialogue && dialogueToReplace != null)
            {
                StartCoroutine(ReplaceDialogueText(dialogueToReplace, text, player));
                return;
            }

            if (replaceDialogue)
            {
                Debug.LogWarning("Cannot replace dialogue: no dialogue bubble has been stored by a #destroy tag. Creating a new bubble instead.");
            }

            GameObject dialogueInstance = Instantiate(prefab, dialoguePanel.transform);

            if (storeDialogueForReplacement)
            {
                dialogueToReplace = dialogueInstance;
            }

            StartCoroutine(DisplayText(dialogueInstance, text, player));
        }
        else if (story.currentChoices.Count > 0)
        {
            DisplayOptions();
        }
        else
        {
            characterSpriteHolder.StartCoroutine(characterSpriteHolder.HideCharacter(false));
            HidePlayerCharacter();
            ClearDialogue();
            Debug.Log("End of story reached.");
        }

    }

    private IEnumerator SceneTransition(string sceneName)
    {
        if (sceneName == "start" || sceneName == "reception" || sceneName == "wedding")
        {
            musicManager.SetLocation(Location.Lobby);
        }
        else if (sceneName == "cafe")
        {
            musicManager.SetLocation(Location.Cafe);
        }
        else if (sceneName == "psychic")
        {
            musicManager.SetLocation(Location.Psychic);
        }
        else if (sceneName == "island")
        {
            musicManager.SetLocation(Location.Beach);
        }

        if (sceneName == "start")
        {
            yield return ReturnToStartScreen();
            yield break;
        }

        if (sceneName == "wedding" || sceneName == "island")
        {
            musicManager.SetWeirdFactor(0);
        }

        if (sceneName == "island")
        {
            musicManager.FadeOutMusic(5f);
        }

        Sprite newBackground;
        Texture2D[] backgroundOverlays;
        bool showPlayerAfterTransition = true;
        // character newCharacter = character.none;
        if (sceneName == "psychic")
        {
            newBackground = psychicBackground;
            backgroundOverlays = psychicBackgroundOverlays;
            // newCharacter = character.owl;
        }
        else if (sceneName == "reception")
        {
            newBackground = recptionBackground;
            backgroundOverlays = receptionBackgroundOverlays;
            // newCharacter = character.doe;
        }
        else if (sceneName == "cafe")
        {
            newBackground = cafeBackground;
            backgroundOverlays = cafeBackgroundOverlays;
            // newCharacter = character.toad1;
        }
        else if (sceneName == "wedding")
        {
            newBackground = weddingBackground;
            backgroundOverlays = null;
            showPlayerAfterTransition = false;
        }
        else if (sceneName == "island")
        {
            newBackground = islandBackground;
            backgroundOverlays = null;
            showPlayerAfterTransition = false;
        }
        else
        {
            Debug.LogWarning($"{sceneName} not implemented");
            isSceneTransitioning = false;
            yield break; 
        }
        yield return new WaitForSeconds(1); // waiting for dialogue line to be displayed
        StartCoroutine(characterSpriteHolder.HideCharacter(false));
        musicManager.PlayCharacterMoveOutSFX();
        HidePlayerCharacter();
        ClearDialogue();
        fadeToBlack.Fade(true);
        yield return new WaitForSeconds(2);
        AssembleBackground(newBackground, backgroundOverlays);
        yield return new WaitForSeconds(0.1f);
        fadeToBlack.Fade(false);
        yield return new WaitForSeconds(1);
        if (showPlayerAfterTransition)
        {
            ShowPlayerCharacter();
            musicManager.PlayCharacterMoveInSFX();
            yield return new WaitForSeconds(1);
        }
        // characterSpriteHolder.ShowCharacter(newCharacter);
        // yield return new WaitForSeconds(0.5f);
        isSceneTransitioning = false;
        DisplayNextLine();

    }

    private IEnumerator ReturnToStartScreen()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(characterSpriteHolder.HideCharacter(false));
        musicManager.PlayCharacterMoveOutSFX();
        HidePlayerCharacter();
        ClearDialogue();
        ClearChoices();
        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        narratorPanel.transform.DOLocalMoveY(-840, 0f);
        choicePanel.transform.DOLocalMoveY(-840, 0f);

        fadeToBlack.Fade(true);
        yield return new WaitForSeconds(2f);

        ClearBackground();
        musicManager.RestoreMusic();
        ResetInkStory();
        startScreen.DOKill();
        startScreen.alpha = 1f;
        startScreen.interactable = true;
        startScreen.blocksRaycasts = true;

        fadeToBlack.Fade(false);
        yield return new WaitForSeconds(2f);
        isSceneTransitioning = false;
    }

    private void ClearBackground()
    {
        background.sprite = null;

        if (backgroundOverlay != null)
        {
            Destroy(backgroundOverlay);
            Destroy(backgroundOverlaySprite);
        }

        backgroundOverlay = null;
        backgroundOverlaySprite = null;
    }

    private void AssembleBackground(Sprite baseBackground, Texture2D[] overlays)
    {
        background.sprite = baseBackground;

        if (backgroundOverlay != null)
        {
            Destroy(backgroundOverlay);
            Destroy(backgroundOverlaySprite);
        }

        int overlayIndex = (currentWeirdFactor - 1) / 2;
        if (currentWeirdFactor <= 0 || overlays == null || overlayIndex >= overlays.Length || overlays[overlayIndex] == null)
        {
            backgroundOverlay = null;
            backgroundOverlaySprite = null;
            return;
        }

        Texture2D overlayTexture = overlays[overlayIndex];
        backgroundOverlaySprite = Sprite.Create(
            overlayTexture,
            new Rect(0f, 0f, overlayTexture.width, overlayTexture.height),
            new Vector2(0.5f, 0.5f),
            100f);

        backgroundOverlay = new GameObject($"Background Overlay {overlayIndex + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform overlayTransform = backgroundOverlay.GetComponent<RectTransform>();
        overlayTransform.SetParent(background.rectTransform, false);
        overlayTransform.anchorMin = Vector2.zero;
        overlayTransform.anchorMax = Vector2.one;
        overlayTransform.offsetMin = Vector2.zero;
        overlayTransform.offsetMax = Vector2.zero;

        Image overlayImage = backgroundOverlay.GetComponent<Image>();
        overlayImage.sprite = backgroundOverlaySprite;
        overlayImage.raycastTarget = false;
    }

    private IEnumerator DisplayNarratorText(string text)
    {
        playerTalkingBounceAnimator?.StopTalkingImmediately();
        characterSpriteHolder.StopTalkingAnimationImmediately();

        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = " ";
        narratorPanel.transform.DOLocalMoveY(-590, 0.5f).SetEase(Ease.OutBack);
        yield return DisplayText(narratorPanel, text, false, 0.5f, false);
    }

    private IEnumerator DisplayText(
        GameObject dialogueInstance,
        string text,
        bool player,
        float initialDelay = 0f,
        bool animateSpeaker = true)
    {
        activeDialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
        activeDialogueRect = dialogueInstance.GetComponent<RectTransform>();
        activeDialogueLine = text;
        skipTypewriter = false;
        isTyping = true;

        activeDialogueText.text = " "; 
        yield return null; 
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueInstance.GetComponent<RectTransform>());

        activeDialogueText.text = "";
        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        if (animateSpeaker && player)
        {
            playerTalkingBounceAnimator?.StartTalking();
        }
        else if (animateSpeaker)
        {
            characterSpriteHolder.StartTalkingAnimation();
            if (currentCharacter == character.owl) musicManager.StartOwlTalk();
            else if (currentCharacter == character.doe) musicManager.StartDoeTalk();
            else if (IsToad(currentCharacter)) musicManager.StartToadTalk();
        }

        foreach (char c in text)
        {
            if (skipTypewriter)
            {
                break;
            }

            activeDialogueText.text += c;
            if (c == '.' || c == '!' || c == '?')
            {
                yield return new WaitForSeconds(0.2f);
            }
            else if (c == ' ')
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

        activeDialogueText.text = text;
        isTyping = false;
        activeDialogueText = null;
        activeDialogueRect = null;
        activeDialogueLine = null;

        if (currentCharacter == character.owl) musicManager.StopOwlTalk();
        else if (currentCharacter == character.doe) musicManager.StopDoeTalk();
        else if (IsToad(currentCharacter)) musicManager.StopToadTalk();

        if (animateSpeaker && player)
        {
            playerTalkingBounceAnimator?.StopTalking();
        }
        else if (animateSpeaker)
        {
            characterSpriteHolder.StopTalkingAnimation();
        }
    }

    private IEnumerator ReplaceDialogueText(GameObject dialogueInstance, string text, bool player)
    {
        activeDialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
        activeDialogueRect = dialogueInstance.GetComponent<RectTransform>();
        activeDialogueLine = text;
        skipTypewriter = false;
        isTyping = true;

        if (player)
        {
            playerTalkingBounceAnimator?.StartTalking();
        }
        else
        {
            characterSpriteHolder.StartTalkingAnimation();
            if (currentCharacter == character.owl) musicManager.StartOwlTalk();
            else if (currentCharacter == character.doe) musicManager.StartDoeTalk(freaky: true);
            else if (IsToad(currentCharacter)) musicManager.StartToadTalk(freaky: true);
        }

        string previousText = activeDialogueText.text;
        for (int length = previousText.Length - 1; length >= 1; length--)
        {
            if (skipTypewriter)
            {
                break;
            }

            activeDialogueText.text = previousText.Substring(0, length);
            RebuildActiveDialogueLayout();
            yield return new WaitForSeconds(0.05f);
        }

        if (!skipTypewriter)
        {
            activeDialogueText.text = text.Substring(0, 1);
            RebuildActiveDialogueLayout();

            for (int index = 1; index < text.Length; index++)
            {
                if (skipTypewriter)
                {
                    break;
                }

                char c = text[index];
                activeDialogueText.text += c;
                if (c == '.' || c == '!' || c == '?')
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else if (c == ' ')
                {
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        activeDialogueText.text = text;
        RebuildActiveDialogueLayout();
        isTyping = false;
        activeDialogueText = null;
        activeDialogueRect = null;
        activeDialogueLine = null;

        if (currentCharacter == character.owl) musicManager.StopOwlTalk();
        else if (currentCharacter == character.doe) musicManager.StopDoeTalk();
        else if (IsToad(currentCharacter)) musicManager.StopToadTalk();

        if (player)
        {
            playerTalkingBounceAnimator?.StopTalking();
        }
        else
        {
            characterSpriteHolder.StopTalkingAnimation();
        }
    }

    private void CompleteActiveDialogueLine()
    {
        skipTypewriter = true;
        if (activeDialogueText != null)
        {
            activeDialogueText.text = activeDialogueLine;
            activeDialogueText.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();

            if (activeDialogueRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(activeDialogueRect);

                if (activeDialogueRect.parent is RectTransform dialogueContainerRect)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContainerRect);
                }
            }
        }
    }

    private bool HasCurrentTag(string tagName)
    {
        return story.currentTags.Exists(tag =>
            string.Equals(tag.Trim(), tagName, StringComparison.OrdinalIgnoreCase));
    }

    private void RebuildActiveDialogueLayout()
    {
        activeDialogueText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(activeDialogueRect);

        if (activeDialogueRect.parent is RectTransform dialogueContainerRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueContainerRect);
        }
    }

    private static bool IsToad(character characterToCheck)
    {
        return characterToCheck == character.toad1
            || characterToCheck == character.toad2
            || characterToCheck == character.toad3;
    }

    private TalkingBounceAnimator GetOrAddTalkingBounceAnimator(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("No target was assigned for talking bounce animation.");
            return null;
        }

        TalkingBounceAnimator animator = target.GetComponent<TalkingBounceAnimator>();
        if (animator == null)
        {
            animator = target.AddComponent<TalkingBounceAnimator>();
        }

        return animator;
    }

    private void SetupPlayerCharacterPanel()
    {
        if (playerCharacterPanel == null)
        {
            Debug.LogWarning("No player character panel is assigned.");
            return;
        }

        playerCharacterVisiblePosition = playerCharacterPanel.transform.localPosition;
        playerCharacterCanvasGroup = playerCharacterPanel.GetComponent<CanvasGroup>();
        if (playerCharacterCanvasGroup == null)
        {
            playerCharacterCanvasGroup = playerCharacterPanel.AddComponent<CanvasGroup>();
        }

        HidePlayerCharacterInstantly();
    }

    private void HidePlayerCharacterInstantly()
    {
        playerTalkingBounceAnimator?.StopTalkingImmediately();

        playerCharacterPanel.transform.localPosition = GetPlayerHiddenPosition();
        playerCharacterCanvasGroup.alpha = 0f;
    }

    private void HidePlayerCharacter()
    {
        if (playerCharacterPanel == null || playerCharacterCanvasGroup == null)
        {
            return;
        }

        playerTalkingBounceAnimator?.StopTalkingImmediately();
        playerCharacterPanel.transform.DOKill();
        playerCharacterCanvasGroup.alpha = 1f;
        playerCharacterPanel.transform.DOLocalMoveX(GetPlayerHiddenPosition().x, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => playerCharacterCanvasGroup.alpha = 0f);
    }

    private void ShowPlayerCharacter()
    {
        if (playerCharacterPanel == null || playerCharacterCanvasGroup == null)
        {
            return;
        }

        playerCharacterPanel.transform.DOKill();
        playerCharacterCanvasGroup.alpha = 1f;
        playerCharacterPanel.transform.DOLocalMoveX(playerCharacterVisiblePosition.x, 0.5f).SetEase(Ease.OutBack);
    }

    private Vector3 GetPlayerHiddenPosition()
    {
        Vector3 hiddenPosition = playerCharacterVisiblePosition;
        hiddenPosition.x -= 1300f;
        return hiddenPosition;
    }

    public void DisplayOptions()
    {
        narratorPanel.transform.DOKill();
        narratorPanel.transform.DOLocalMoveY(-840, 0.5f).SetEase(Ease.InBack);

        if (choicePanel.GetComponentsInChildren<Button>().Length > 0) return;

        choicePanel.transform.DOLocalMoveY(-590, 0.5f).SetEase(Ease.OutBack);

        if (story.currentChoices.Count > 0)
        {
            foreach (var choice in story.currentChoices)
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choicePanel.transform);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceInstance.GetComponent<RectTransform>());
                choiceInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClickChoice(choice));
                ConfigureCardChoiceHover(choiceInstance, choice);
                ConfigureTestChoiceHover(choiceInstance, choice);
            }
        }
    }

    private void ConfigureCardChoiceHover(GameObject choiceInstance, Choice choice)
    {
        if (choice.tags == null || cardGame == null)
        {
            return;
        }

        int third = -1;
        if (choice.tags.Contains("cards:hoverLeft")) third = 0;
        else if (choice.tags.Contains("cards:hoverMiddle")) third = 1;
        else if (choice.tags.Contains("cards:hoverRight")) third = 2;

        if (third < 0)
        {
            return;
        }

        EventTrigger trigger = choiceInstance.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = choiceInstance.AddComponent<EventTrigger>();
        }

        int capturedThird = third;
        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => cardGame.PreviewCardThird(capturedThird));
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => cardGame.StopPreviewingCard(capturedThird));
        trigger.triggers.Add(exit);
    }

    private void ConfigureTestChoiceHover(GameObject choiceInstance, Choice choice)
    {
        if (choice.tags == null)
        {
            return;
        }
        EventTrigger trigger = choiceInstance.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = choiceInstance.AddComponent<EventTrigger>();
        }

        if (choice.tags.Contains("test:hoverPlane"))
        {
            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ =>
            {
                musicManager.PlayTestHoverSFX();
                rorschachTest.FadeImageB();
            });
            trigger.triggers.Add(enter);
        }
        else if (choice.tags.Contains("test:hoverPuppy"))
        {
            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ =>
            {
                musicManager.PlayTestHoverSFX();
                rorschachTest.FadeImageA();
            });
            trigger.triggers.Add(enter);
        }
    }

    public void OnClickChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicePanel.transform.DOLocalMoveY(-840, 0.5f).SetEase(Ease.InBack);
        ClearChoices();
        DisplayNextLine();
        musicManager.PlayClickSFX();
    }

    public void ClearChoices()
    {
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearDialogue()
    {
        dialogueToReplace = null;

        foreach (Transform child in dialoguePanel.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }


    // public void OnClickContinue()
    // {
    //     DisplayNextLine();
    // }


}
