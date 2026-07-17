using UnityEngine;
using DG.Tweening;
using System.Collections;
// using Unity.VisualScripting;
using UnityEngine.UI;

public enum character
{
    doe,
    owl,
    toad,
    none
}

public enum expression
{
    neutral,
    smile,
    frown,
    notes
}
[System.Serializable]
public struct expressionPair
{
    public expression characterExpression;
    public Sprite characterSprite;
}

[System.Serializable]
public struct characterExpressionPair
{
    public character character;
    public GameObject characterPanel;
    public expressionPair[] expressionPairs;
}
public class CharacterSpriteHolder : MonoBehaviour
{
    [SerializeField] private characterExpressionPair[] characterExpressionPairs;
    private character currentCharacter;
    private Sequence talkAnimation;
    void Start()
    {
        StartCoroutine(HideCharacter());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowCharacter(character.doe);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(HideCharacter(false));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SetExpression(expression.neutral));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SetExpression(expression.smile));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SetExpression(expression.frown));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SetExpression(expression.notes));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            StartTalkingAnimation();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StopTalkingAnimation();
        }
    }

    public void ShowCharacter(character character)
    {
        foreach (var characterPair in characterExpressionPairs)
        {
            if (characterPair.character == character)
            {
                characterPair.characterPanel.GetComponent<CanvasGroup>().alpha = 1f;
                currentCharacter = character;
            }
        }
        transform.DOLocalMoveX(600, 0.5f).SetEase(Ease.OutBack);
    }

    public IEnumerator HideCharacter(bool instant = true)
    {
        currentCharacter = character.none;
        float transitionTime = 0.5f;
        if (instant)
        {
            transitionTime = 0f;
        }

        transform.DOLocalMoveX(1300, transitionTime).SetEase(Ease.InBack);

        yield return new WaitForSeconds(transitionTime);
        foreach (var characterPair in characterExpressionPairs)
        {
            characterPair.characterPanel.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public void StartTalkingAnimation(bool player = false)
    {
        GameObject target = transform.gameObject; // Use the current GameObject for the talking animation
        float originalY = target.transform.localPosition.y;

        talkAnimation = DOTween.Sequence();
        int random = UnityEngine.Random.Range(3, 7);
        int value = UnityEngine.Random.value > 0.5f ? random : -random;
        talkAnimation.Append(target.transform.DOLocalRotate(new Vector3(0, 0, value), 1f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine));

        talkAnimation.Insert(0,
        target.transform.DOLocalMoveY(originalY - 10, 0.2f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine));
    }

    public void StopTalkingAnimation()
    {
        if (talkAnimation != null)
        {
            talkAnimation.Kill();
            talkAnimation = null;
        }
        // reset character position and rotations
        transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        transform.DOLocalMoveY(-590, 0.5f);
    }

    public IEnumerator SetExpression(expression expression)
    {
        if (currentCharacter == character.none)
        {
            Debug.LogWarning("No character is currently displayed.");
            yield return null;
        }
        else
        {
            foreach (var characterPair in characterExpressionPairs)
            {
                if (characterPair.character == currentCharacter)
                {
                    foreach (var expressionPair in characterPair.expressionPairs)
                    {
                        if (expressionPair.characterExpression == expression)
                        {
                            characterPair.characterPanel.GetComponentInChildren<Image>().sprite = expressionPair.characterSprite;
                            yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds before continuing
                            // Optionally, you can change back to neutral expression after the wait
                            foreach (var neutralPair in characterPair.expressionPairs)
                            {
                                if (neutralPair.characterExpression == expression.neutral)
                                {
                                    characterPair.characterPanel.GetComponentInChildren<Image>().sprite = neutralPair.characterSprite;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
