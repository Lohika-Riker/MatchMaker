using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class InkManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJsonAsset;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject choicePanel;
    private Story story;
    [SerializeField] private GameObject dialoguePrefabPlayer, dialoguePrefabOther;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private GameObject otherCharacterPanel;
    [SerializeField] private GameObject continueButton;

    void Start()
    {
        StartStory();
    }

    private void StartStory()
    {
        story = new Story(inkJsonAsset.text);
        
        ClearDialogue();
        ClearChoices();
        otherCharacterPanel.transform.DOLocalMoveX(1300, 0f);
        choicePanel.transform.DOLocalMoveY(-840, 0f);
        continueButton.SetActive(false);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
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
                        otherCharacterPanel.transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.OutBack);
                        DisplayNextLine();
                        return;
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

            if (story.currentTags.Contains("player"))
            {
                prefab = dialoguePrefabPlayer;
            }
            else
            {
                prefab = dialoguePrefabOther;
            }

            GameObject dialogueInstance = Instantiate(prefab, dialoguePanel.transform);
            dialogueInstance.GetComponentInChildren<TextMeshProUGUI>().text = text; // sets the current text to the dialogue instance

            StartCoroutine(DisplayText(dialogueInstance));
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

    private IEnumerator DisplayText(GameObject dialogueInstance)
    {
        yield return null; // Wait for one frame to ensure the UI is updated
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueInstance.GetComponent<RectTransform>());
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds before showing the continue button
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
