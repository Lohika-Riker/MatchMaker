using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private float angle = 7f;
    [SerializeField] private float gap = 10f;
    [SerializeField] private int cardsCount = 22;

    [Header("Fan Animation")]
    [SerializeField] private float fanDuration = 0.1f;

    // create an array to store the cards
    private Card [] cards;
    int cardNumber;
    private int cardCounter = 0;
    private Sequence fanSequence;
    private bool isDeckAnimating;
    private bool isCollapsed;
    private readonly int[] previewCardIndices = { -1, -1, -1 };
    private int activePreviewThird = -1;

    void Start()
    {
        cards = System.Array.Empty<Card>();
    }


    void Update()
    {
        if (isDeckAnimating)
        {
            return;
        }

        // DEBUG 
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     GenerateDeck();
        //     return;
        // }

        // if (!HasDeck())
        // {
        //     return;
        // }

        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     cardNumber = Random.Range(0,cardsCount);
        //     if (cards[cardNumber].SelectCard(cardCounter))
        //     {
        //         cardCounter++;
        //     }
        // }
        // else if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     DiscardSelectedCard();
        // }
    }

    public void GenerateDeck()
    {
        if (isDeckAnimating || HasDeck())
        {
            return;
        }

        cards = new Card[cardsCount];
        for (int i = 0; i < previewCardIndices.Length; i++)
        {
            previewCardIndices[i] = -1;
        }
        activePreviewThird = -1;
        cardNumber = Random.Range(0, cardsCount);
        isCollapsed = false;
        StartCoroutine(GenerateDeckRoutine());
    }

    private IEnumerator GenerateDeckRoutine()
    {
        fanSequence?.Kill();
        isDeckAnimating = true;
        musicManager.PlayDeckFanSFX();

        for (int i = 0; i < cardsCount; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            RectTransform cardRect = instance.GetComponent<RectTransform>();

            float offsetFromCenter = i - ((cardsCount - 1) * 0.5f);

            Vector2 targetPosition = cardRect.anchoredPosition;
            targetPosition.x = offsetFromCenter * gap;
            Vector3 targetRotation = new Vector3(0f, 0f, -offsetFromCenter * angle);

            // Each card enters from the previous card's pose, then advances one
            // position along the fan. The first card begins in its final slot.
            float previousOffset = i == 0
                ? offsetFromCenter
                : (i - 1) - ((cardsCount - 1) * 0.5f);

            Vector2 startPosition = cardRect.anchoredPosition;
            startPosition.x = previousOffset * gap;
            cardRect.anchoredPosition = startPosition;
            cardRect.localRotation = Quaternion.Euler(
                0f,
                0f,
                -previousOffset * angle);

            cards[i] = instance.GetComponent<Card>();

            // The first card is already in its final pose. Every following card
            // must finish moving before the next card is instantiated.
            if (i == 0)
            {
                continue;
            }

            fanSequence = DOTween.Sequence()
                .Join(cardRect.DOAnchorPos(targetPosition, fanDuration).SetEase(Ease.OutCubic))
                .Join(cardRect.DOLocalRotate(targetRotation, fanDuration).SetEase(Ease.OutCubic));

            yield return fanSequence.WaitForCompletion();
        }

        isDeckAnimating = false;
    }

    public void CollapseDeck()
    {
        if (isDeckAnimating || isCollapsed || !HasDeck())
        {
            return;
        }

        StartCoroutine(CollapseDeckRoutine());
    }

    public void DiscardSelectedCard()
    {
        if (isDeckAnimating || !HasDeck()
            || cardNumber < 0 || cardNumber >= cards.Length
            || cards[cardNumber] == null || !cards[cardNumber].IsSelected)
        {
            return;
        }

        cards[cardNumber].DiscardCard();
        CollapseDeck();
    }

    public void SelectLeftCard()
    {
        SelectRandomCardFromThird(0);
    }

    public void SelectMiddleCard()
    {
        SelectRandomCardFromThird(1);
    }

    public void SelectRightCard()
    {
        SelectRandomCardFromThird(2);
    }

    private void SelectRandomCardFromThird(int third)
    {
        if (isDeckAnimating || !HasDeck())
        {
            return;
        }

        int startIndex = cards.Length * third / 3;
        int endIndex = cards.Length * (third + 1) / 3;
        int availableCards = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (cards[i] != null && !cards[i].IsSelected)
            {
                availableCards++;
            }
        }

        if (availableCards == 0)
        {
            Debug.LogWarning($"There are no unselected cards left in deck third {third + 1}.");
            return;
        }

        int previewIndex = previewCardIndices[third];
        if (previewIndex >= startIndex && previewIndex < endIndex
            && cards[previewIndex] != null && !cards[previewIndex].IsSelected
            && cards[previewIndex].SelectCard(cardCounter))
        {
            cardNumber = previewIndex;
            cardCounter++;
            activePreviewThird = -1;
            return;
        }

        int randomAvailableIndex = Random.Range(0, availableCards);

        for (int i = startIndex; i < endIndex; i++)
        {
            if (cards[i] == null || cards[i].IsSelected)
            {
                continue;
            }

            if (randomAvailableIndex-- == 0 && cards[i].SelectCard(cardCounter))
            {
                cardNumber = i;
                cardCounter++;
                return;
            }
        }
    }

    public void PreviewCardThird(int third)
    {
        if (third < 0 || third > 2 || isDeckAnimating || !HasDeck())
        {
            return;
        }

        if (activePreviewThird >= 0 && activePreviewThird != third)
        {
            StopPreviewingCard(activePreviewThird);
        }

        int startIndex = cards.Length * third / 3;
        int endIndex = cards.Length * (third + 1) / 3;
        int index = previewCardIndices[third];

        if (index < startIndex || index >= endIndex
            || cards[index] == null || cards[index].IsSelected)
        {
            int randomOffset = Random.Range(0, endIndex - startIndex);
            index = -1;

            for (int offset = 0; offset < endIndex - startIndex; offset++)
            {
                int candidate = startIndex + ((randomOffset + offset) % (endIndex - startIndex));
                if (cards[candidate] != null && !cards[candidate].IsSelected)
                {
                    index = candidate;
                    break;
                }
            }

            if (index < 0)
            {
                return;
            }

            previewCardIndices[third] = index;
        }

        cards[index].RaiseCard();
        activePreviewThird = third;
    }

    public void StopPreviewingCard(int third)
    {
        if (third < 0 || third > 2)
        {
            return;
        }

        int index = previewCardIndices[third];
        if (HasDeck() && index >= 0 && index < cards.Length && cards[index] != null)
        {
            cards[index].LowerCard();
        }

        if (activePreviewThird == third)
        {
            activePreviewThird = -1;
        }
    }

    private IEnumerator CollapseDeckRoutine()
    {
        isDeckAnimating = true;
        musicManager.PlayDeckCollapseSFX();

        // Move each card onto its left neighbour and remove it once it lands.
        for (int slot = cardsCount - 1; slot > 0; slot--)
        {
            float previousOffset = (slot - 1) - ((cardsCount - 1) * 0.5f);
            Vector3 targetRotation = new Vector3(0f, 0f, -previousOffset * angle);
            RectTransform cardRect = (RectTransform)cards[slot].transform;
            Vector2 targetPosition = cardRect.anchoredPosition;
            targetPosition.x = previousOffset * gap;

            fanSequence = DOTween.Sequence()
                .Join(
                    cardRect.DOAnchorPos(targetPosition, fanDuration).SetEase(Ease.InOutCubic));
            fanSequence.Join(
                cardRect.DOLocalRotate(targetRotation, fanDuration).SetEase(Ease.InOutCubic));

            yield return fanSequence.WaitForCompletion();

            Destroy(cards[slot].gameObject);
            cards[slot] = null;
        }

        Destroy(cards[0].gameObject);
        cards[0] = null;
        cards = System.Array.Empty<Card>();
        isCollapsed = true;
        isDeckAnimating = false;
    }

    private bool HasDeck()
    {
        return cards != null
            && cards.Length == cardsCount
            && cards.Length > 0
            && cards[0] != null;
    }

    private void OnDestroy()
    {
        fanSequence?.Kill();
    }
}
