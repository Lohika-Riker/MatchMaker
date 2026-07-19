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
    [SerializeField] private TextAsset inkJsonAsset;
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
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private RorschachTest rorschachTest;
    private TalkingBounceAnimator playerTalkingBounceAnimator;
    private CanvasGroup playerCharacterCanvasGroup;
    private Vector3 playerCharacterVisiblePosition;
    private bool isSceneTransitioning;
    private bool isTyping;
    private bool skipTypewriter;
    private TextMeshProUGUI activeDialogueText;
    private RectTransform activeDialogueRect;
    private string activeDialogueLine;
    private character currentCharacter;

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
        StartStory();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("click to continue");
            DisplayNextLine();
        }
    }

    private void StartStory()
    {
        story = new Story(inkJsonAsset.text);

        ClearDialogue();
        ClearChoices();
        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        narratorPanel.transform.DOLocalMoveY(-840, 0f);
        choicePanel.transform.DOLocalMoveY(-840, 0f);
        // continueButton.SetActive(false);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
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

            // Handle cleanup before tags such as scene/entrance can return early.
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
                    if (parts[1] == "deer")
                    {
                        characterSpriteHolder.ShowCharacter(character.doe);
                        currentCharacter = character.doe;
                        // DisplayNextLine();
                        return;
                    }
                    else if (parts[1] == "owl")
                    {
                        characterSpriteHolder.ShowCharacter(character.owl);
                        currentCharacter = character.owl;
                        // DisplayNextLine();
                        return;
                    }
                    else if (parts[1] == "toad" || parts[1] == "toad1")
                    {
                        characterSpriteHolder.ShowCharacter(character.toad1);
                        currentCharacter = character.toad1;
                        // DisplayNextLine();
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
                        // DisplayNextLine();
                        return;
                    }
                }
                else if (parts[0] == "exp")
                {
                    print($"change {currentCharacter}'s expression to {parts[1]}");
                    // catch any errors that might occur during the parsing of the expression
                    try
                    {
                        characterSpriteHolder.StartCoroutine(characterSpriteHolder.SetExpression((expression)Enum.Parse(typeof(expression), parts[1])));
                    }
                    catch (ArgumentException e)
                    {
                        Debug.LogError($"Invalid expression '{parts[1]}' for character '{currentCharacter}'. Please check the Ink script and ensure the expression is defined correctly. Error: {e.Message}");
                        // characterSpriteHolder.StartCoroutine(characterSpriteHolder.SetExpression((expression)Enum.Parse(typeof(expression), parts[1])));
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
                    if (cardGame != null)
                    {
                        switch (parts[1])
                        {
                            case "fan":
                                cardGame.GenerateDeck();
                                break;
                            case "selectLeft":
                                cardGame.SelectLeftCard();
                                break;
                            case "selectMiddle":
                                cardGame.SelectMiddleCard();
                                break;
                            case "selectRight":
                                cardGame.SelectRightCard();
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
                    return;
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

            // dialogue speech bubble
            if (text == null || text == "")
            {
                print("No text to display.");
                // continueButton.SetActive(true);
                return;
            }
            GameObject prefab;

            bool player = false;
            if (story.currentTags.Contains("player"))
            {
                prefab = dialoguePrefabPlayer;
                player = true;
            }
            else if (story.currentTags.Contains("narrator"))
            {
                // Handle narrator dialogue
                StartCoroutine(DisplayNarratorText(text));
                return;
            }
            else
            {
                prefab = dialoguePrefabOther;
            }

            GameObject dialogueInstance = Instantiate(prefab, dialoguePanel.transform);

            StartCoroutine(DisplayText(dialogueInstance, text, player));
        }
        else if (story.currentChoices.Count > 0)
        {
            DisplayOptions();
        }
        else
        {
            Debug.Log("End of story reached.");
            characterSpriteHolder.StartCoroutine(characterSpriteHolder.HideCharacter(false));
            HidePlayerCharacter();
            ClearDialogue();
        }

    }

    private IEnumerator SceneTransition(string sceneName)
    {
        Sprite newBackground = null;
        character newCharacter = character.none;
        if (sceneName == "psychic")
        {
            newBackground = psychicBackground;
            newCharacter = character.owl;
        }
        else if (sceneName == "reception")
        {
            newBackground = recptionBackground;
            newCharacter = character.doe;
        }
        else if (sceneName == "cafe")
        {
            newBackground = cafeBackground;
            newCharacter = character.toad1;
        }
        else
        {
            Debug.LogWarning($"{sceneName} not implemented");
            isSceneTransitioning = false;
            yield break; 
        }
        yield return new WaitForSeconds(1); // waiting for dialogue line to be displayed
        StartCoroutine(characterSpriteHolder.HideCharacter(false));
        HidePlayerCharacter();
        ClearDialogue();
        fadeToBlack.Fade(true);
        yield return new WaitForSeconds(2);
        background.sprite = newBackground;
        yield return new WaitForSeconds(0.1f);
        fadeToBlack.Fade(false);
        yield return new WaitForSeconds(1);
        ShowPlayerCharacter();
        yield return new WaitForSeconds(1);
        // characterSpriteHolder.ShowCharacter(newCharacter);
        // yield return new WaitForSeconds(0.5f);
        isSceneTransitioning = false;
        DisplayNextLine();

    }

    private IEnumerator DisplayNarratorText(string text)
    {
        // Narration has no speaker, so make sure neither character carries a
        // talking animation into the narrator line.
        playerTalkingBounceAnimator?.StopTalkingImmediately();
        characterSpriteHolder.StopTalkingAnimationImmediately();

        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = " "; // sets the current text to the dialogue instance
        // slide in narrator panel
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

        activeDialogueText.text = " "; // sets the current text to the dialogue instance
        yield return null; // Wait for one frame to ensure the UI is updated
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
                yield return new WaitForSeconds(0.2f); // Add a longer pause after punctuation
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
        // continueButton.SetActive(true);
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
        if (choicePanel.GetComponentsInChildren<Button>().Length > 0) return;

        // continueButton.SetActive(false);
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
        foreach (Transform child in dialoguePanel.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }


    public void OnClickContinue()
    {
        DisplayNextLine();
    }


}
