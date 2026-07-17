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
    private const float VisibleX = 600f;
    private const float HiddenX = 1300f;
    private const float TransitionTime = 0.5f;

    [SerializeField] private characterExpressionPair[] characterExpressionPairs;
    private character currentCharacter;
    private TalkingBounceAnimator talkingBounceAnimator;
    private Sequence characterTransition;

    void Awake()
    {
        talkingBounceAnimator = GetComponent<TalkingBounceAnimator>();
        if (talkingBounceAnimator == null)
        {
            talkingBounceAnimator = gameObject.AddComponent<TalkingBounceAnimator>();
        }

        SetHiddenInstantly();
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
        if (!HasCharacter(character))
        {
            Debug.LogWarning($"No character panel is configured for {character}.");
            return;
        }

        talkingBounceAnimator.StopTalkingImmediately();
        characterTransition?.Kill();
        transform.DOKill();

        if (currentCharacter != character.none && currentCharacter != character)
        {
            characterTransition = DOTween.Sequence()
                .Append(transform.DOLocalMoveX(HiddenX, TransitionTime).SetEase(Ease.InBack))
                .AppendCallback(() => SetVisibleCharacter(character))
                .Append(transform.DOLocalMoveX(VisibleX, TransitionTime).SetEase(Ease.OutBack));

            return;
        }

        SetVisibleCharacter(character);
        transform.DOLocalMoveX(VisibleX, TransitionTime).SetEase(Ease.OutBack);
    }

    public IEnumerator HideCharacter(bool instant = true)
    {
        talkingBounceAnimator.StopTalkingImmediately();
        characterTransition?.Kill();

        currentCharacter = character.none;
        float transitionTime = 0.5f;
        if (instant)
        {
            transitionTime = 0f;
        }

        transform.DOLocalMoveX(HiddenX, transitionTime).SetEase(Ease.InBack);

        yield return new WaitForSeconds(transitionTime);
        foreach (var characterPair in characterExpressionPairs)
        {
            characterPair.characterPanel.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    private void SetHiddenInstantly()
    {
        currentCharacter = character.none;
        Vector3 hiddenPosition = transform.localPosition;
        hiddenPosition.x = HiddenX;
        transform.localPosition = hiddenPosition;

        foreach (var characterPair in characterExpressionPairs)
        {
            characterPair.characterPanel.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    private bool HasCharacter(character character)
    {
        foreach (var characterPair in characterExpressionPairs)
        {
            if (characterPair.character == character)
            {
                return true;
            }
        }

        return false;
    }

    private void SetVisibleCharacter(character character)
    {
        foreach (var characterPair in characterExpressionPairs)
        {
            CanvasGroup canvasGroup = characterPair.characterPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = characterPair.character == character ? 1f : 0f;
        }

        currentCharacter = character;
    }

    public void StartTalkingAnimation(bool player = false)
    {
        talkingBounceAnimator.StartTalking();
    }

    public void StopTalkingAnimation()
    {
        talkingBounceAnimator.StopTalking();
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
