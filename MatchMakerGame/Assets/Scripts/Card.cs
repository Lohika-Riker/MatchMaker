using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [Header("Selection Animation")]
    [SerializeField] private Graphic cardImage;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private float selectionDuration = 1f;
    [SerializeField] private Sprite hangedManCard, loversCard;

    private bool raised = false;
    private bool selected = false;
    private float distance = 70f;
    private Sequence selectionSequence;
    private Tween previewTween;
    private Vector2 restingAnchoredPosition;
    private bool hasRestingAnchoredPosition;
    private int selectedNumber;

    public bool IsSelected => selected;

    private void Awake()
    {
        if (cardImage == null)
        {
            cardImage = transform.childCount > 0
                ? transform.GetChild(0).GetComponentInChildren<Graphic>()
                : GetComponent<Graphic>();
        }
    }

    public void DiscardCard()
    {
        if (!selected)
        {
            print("can't discard a card that is not selected");
            return;
        }
        // move card off screen
        transform.DOLocalMoveX(1200, 1f).SetEase(Ease.OutBack);
    }

    public bool SelectCard(int select)
    {
        if (selected)
        {
            return false;
        }

        selectedNumber = select;

        selected = true;
        raised = false;
        previewTween?.Kill();

        // UI elements later in the panel hierarchy render on top of earlier siblings.
        transform.SetAsLastSibling();

        RectTransform cardRect = (RectTransform)transform;
        Vector2 centeredPosition = GetCenteredPosition(cardRect);
        centeredPosition.y = -100;

        selectionSequence?.Kill();
        selectionSequence = DOTween.Sequence()
            .Join(cardRect.DOAnchorPos(centeredPosition, selectionDuration).SetEase(Ease.InOutQuad))
            .Join(cardRect.DOLocalRotate(
                new Vector3(0f, 180f, 0f),
                selectionDuration,
                RotateMode.FastBeyond360).SetEase(Ease.InOutQuad))
            .InsertCallback(selectionDuration * 0.5f, ChangeCardImage);

        return true;
    }

    private static Vector2 GetCenteredPosition(RectTransform cardRect)
    {
        // Offset the pivot so the visual centre of the card, rather than its pivot,
        // lands in the centre of its parent RectTransform.
        return new Vector2(
            (cardRect.pivot.x - 0.5f) * cardRect.rect.width,
            (cardRect.pivot.y - 0.5f) * cardRect.rect.height);
    }

    private void ChangeCardImage()
    {
        if (cardImage != null)
        {
            // TODO; change sprite 
            cardImage.color = selectedColor;
            if (selectedNumber == 0)
            {
                cardImage.GetComponent<Image>().sprite = hangedManCard;
            }
            else
            {
                cardImage.GetComponent<Image>().sprite = loversCard;
            }
        }
    }

    private void OnDestroy()
    {
        previewTween?.Kill();
        selectionSequence?.Kill();
    }

    public void RaiseCard()
    {
        if (selected) return;
        if (raised) return;

        RectTransform cardRect = (RectTransform)transform;
        if (!hasRestingAnchoredPosition)
        {
            restingAnchoredPosition = cardRect.anchoredPosition;
            hasRestingAnchoredPosition = true;
        }

        Vector2 raiseOffset = cardRect.localRotation * (Vector3.up * distance);
        previewTween?.Kill();
        previewTween = cardRect.DOAnchorPos(restingAnchoredPosition + raiseOffset, 0.2f);
        raised = true;
    }

    public void LowerCard()
    {
        if (selected) return;
        if (!raised) return;

        RectTransform cardRect = (RectTransform)transform;
        previewTween?.Kill();
        previewTween = cardRect.DOAnchorPos(restingAnchoredPosition, 0.2f);
        raised = false;
    }


}
