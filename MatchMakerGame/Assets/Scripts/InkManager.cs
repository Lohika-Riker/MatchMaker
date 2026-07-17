using UnityEngine;
using TMPro;
using Ink.Runtime;


public class InkManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJsonAsset;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject choicePanel;
    private Story story;
    [SerializeField] private GameObject dialoguePrefab;
    [SerializeField] private GameObject choicePrefab;

    void Start()
    {
        StartStory();
    }

    private void StartStory()
    {
        story = new Story(inkJsonAsset.text);
        foreach (Transform child in dialoguePanel.transform)
        {
            Destroy(child.gameObject);
        }
        ClearChoices();
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
            ClearChoices();
            string text = story.Continue(); 
            text = text?.Trim(); 

            GameObject dialogueInstance = Instantiate(dialoguePrefab, dialoguePanel.transform);
            dialogueInstance.GetComponentInChildren<TextMeshProUGUI>().text = text; // sets the current text to the dialogue instance
        }
        else if (story.currentChoices.Count > 0)
        {
            DisplayOptions();
        }
        else
        {
            Debug.Log("End of story reached.");
        }
        
    }

    public void DisplayOptions()
    {
        if (story.currentChoices.Count > 0)
        {
            foreach (var choice in story.currentChoices)
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choicePanel.transform);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
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
        DisplayNextLine();
    }
    
    public void ClearChoices()
    {
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    

    public void OnClickContinue()
    {
        DisplayNextLine();
    }


}
