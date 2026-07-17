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
    
    private character currentCharacter;

    [SerializeField] private expressionPair[] doeExpressionPairs, owlExpressionPairs, toadExpressionPairs;

    void Start()
    {
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
                }
                else if (parts[0] == "exp")
                {
                    print($"change {currentCharacter}'s expression to {parts[1]}");
                    characterSpriteHolder.StartCoroutine(characterSpriteHolder.SetExpression((expression)Enum.Parse(typeof(expression), parts[1])));
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
        characterSpriteHolder.StartTalkingAnimation();

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
        characterSpriteHolder.StopTalkingAnimation();
        continueButton.SetActive(true);
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
