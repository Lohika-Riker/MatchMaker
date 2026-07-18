using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
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
    [SerializeField] private GameObject continueButton;
    [SerializeField] private CharacterSpriteHolder characterSpriteHolder;
    private TalkingBounceAnimator playerTalkingBounceAnimator;
    private CanvasGroup playerCharacterCanvasGroup;
    private Vector3 playerCharacterVisiblePosition;
    
    private character currentCharacter;

    // [SerializeField] private expressionPair[] doeExpressionPairs, owlExpressionPairs, toadExpressionPairs;

    void Start()
    {
        playerTalkingBounceAnimator = GetOrAddTalkingBounceAnimator(playerCharacterPanel);
        SetupPlayerCharacterPanel();
        StartStory();
    }

    private void StartStory()
    {
        story = new Story(inkJsonAsset.text);
        
        ClearDialogue();
        ClearChoices();
        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        narratorPanel.transform.DOLocalMoveY(-840, 0f);
        choicePanel.transform.DOLocalMoveY(-840, 0f);
        continueButton.SetActive(false);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
            narratorPanel.transform.DOLocalMoveY(-840, 0.5f).SetEase(Ease.OutBack);
            string text = story.Continue(); 
            text = text?.Trim(); 

            // character entrance

            foreach (var tag in story.currentTags)
            {
                string[] parts = tag.Split(':');

                if (parts[0] == "entrance")
                {
                    if (parts[1] == "deer")
                    {
                        characterSpriteHolder.ShowCharacter(character.doe);
                        DisplayNextLine();
                        return;
                    }
                    else if (parts[1] == "owl")
                    {
                        characterSpriteHolder.ShowCharacter(character.owl);
                        DisplayNextLine();
                        return;
                    }
                    else if (parts[1] == "toad")
                    {
                        characterSpriteHolder.ShowCharacter(character.toad);
                        DisplayNextLine();
                        return;
                    }
                    else if (parts[1] == "player")
                    {
                        ShowPlayerCharacter();
                        DisplayNextLine();
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
            }

            // dialogue speech bubble
            if (text == null || text == "")
            {
                print("No text to display.");
                continueButton.SetActive(true);
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
            continueButton.SetActive(false);
            // otherCharacterPanel.transform.DOLocalMoveX(1300, 0.5f).SetEase(Ease.OutBack);
            characterSpriteHolder.StartCoroutine(characterSpriteHolder.HideCharacter(false));
            HidePlayerCharacter();
            ClearDialogue();
        }
        
    }

    private IEnumerator DisplayNarratorText(string text)
    {
        narratorPanel.GetComponentInChildren<TextMeshProUGUI>().text = " "; // sets the current text to the dialogue instance
        // slide in narrator panel
        narratorPanel.transform.DOLocalMoveY(-590, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f); // Wait for the narrator panel to finish sliding in
        StartCoroutine(DisplayText(narratorPanel, text, false));
        yield return null;
    }

    private IEnumerator DisplayText(GameObject dialogueInstance, string text, bool player)
    {
        dialogueInstance.GetComponentInChildren<TextMeshProUGUI>().text = " "; // sets the current text to the dialogue instance
        yield return null; // Wait for one frame to ensure the UI is updated
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueInstance.GetComponent<RectTransform>());
        
        dialogueInstance.GetComponentInChildren<TextMeshProUGUI>().text = "";
        if (player)
        {
            playerTalkingBounceAnimator?.StartTalking();
        }
        else
        {
            characterSpriteHolder.StartTalkingAnimation();
        }

        foreach(char c in text)
        {
            dialogueInstance.GetComponentInChildren<TextMeshProUGUI>().text += c;
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

        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds before showing the continue button
        if (player)
        {
            playerTalkingBounceAnimator?.StopTalking();
        }
        else
        {
            characterSpriteHolder.StopTalkingAnimation();
        }
        continueButton.SetActive(true);
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

        continueButton.SetActive(false);
        choicePanel.transform.DOLocalMoveY(-590, 0.5f).SetEase(Ease.OutBack);

        if (story.currentChoices.Count > 0)
        {
            foreach (var choice in story.currentChoices)
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choicePanel.transform);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceInstance.GetComponent<RectTransform>());
                choiceInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClickChoice(choice));
            }
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
            Destroy(child.gameObject);
        }
    }
    

    public void OnClickContinue()
    {
        DisplayNextLine();
    }


}
