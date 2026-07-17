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
    [SerializeField] private GameObject otherCharacterPanel, playerCharacterPanel;
    [SerializeField] private GameObject continueButton;
    public enum character
    {
        doe,
        owl,
        toad
    }
    private character currentCharacter;

    public enum expression
    {
        neutral,
        smile,
        frown,
        notes
    }

    [Serializable]
    public struct expressionPair
    {
        public expression characterExpression;
        public Sprite characterSprite;
    }
    [SerializeField] private expressionPair[] expressionPairs;

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
        otherCharacterPanel.transform.DOLocalMoveX(1300, 0f);
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
                        currentCharacter = character.doe;
                        otherCharacterPanel.transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.OutBack);
                        DisplayNextLine();
                        return;
                    }
                }
                else if (parts[0] == "exp")
                {
                    print($"change {currentCharacter}'s expression to {parts[1]}");
                    StartCoroutine(ChangeExpression(currentCharacter, (expression)Enum.Parse(typeof(expression), parts[1])));
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
                // Handle narrator dialogue if needed
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
            otherCharacterPanel.transform.DOLocalMoveX(1300, 0.5f).SetEase(Ease.OutBack);
            ClearDialogue();
        }
        
    }

    private IEnumerator ChangeExpression(character character, expression newExpression)
    {
        Sprite newSprite = null;
        foreach (var pair in expressionPairs)
        {
            if (pair.characterExpression == newExpression)
            {
                newSprite = pair.characterSprite;
                break;
            }
        }

        if (newSprite != null)
        {
            switch (character)
            {
                case character.doe:
                    otherCharacterPanel.GetComponent<Image>().sprite = newSprite;
                    break;
                case character.owl:
                    // Change owl's sprite
                    break;
                case character.toad:
                    // Change toad's sprite
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"No sprite found for expression {newExpression}");
        }

        yield return new WaitForSeconds(1.5f); // Wait for 0.5 seconds before continuing
        // change back to neutral expression
        otherCharacterPanel.GetComponent<Image>().sprite = expressionPairs[0].characterSprite; // Assuming the first pair is neutral
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
        // Tween talkAnim = null;
        Sequence otherTalkAnimation = DOTween.Sequence();
        GameObject target = null;
        int originalY = 0;
        if (player)
        {
            target = playerCharacterPanel;
            originalY = -440;
        }
        else
        {
            target = otherCharacterPanel;
            originalY = -590;
        }

         // animate other character for duration of typewriter text
            int random = UnityEngine.Random.Range(3, 7);
            int value = UnityEngine.Random.value > 0.5f ? random : -random;
            otherTalkAnimation.Append(target.transform.DOLocalRotate(new Vector3(0, 0, value), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));

            // add bobbing animation to other character panel
            otherTalkAnimation.Insert(0, 
            target.transform.DOLocalMoveY(originalY - 10, 0.2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)); 

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
        otherTalkAnimation.Kill(); // stop the talk animation
        // reset character position and rotations
        target.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        otherCharacterPanel.transform.DOLocalMoveY(-590, 0.5f);
        playerCharacterPanel.transform.DOLocalMoveY(-440, 0.5f);
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
