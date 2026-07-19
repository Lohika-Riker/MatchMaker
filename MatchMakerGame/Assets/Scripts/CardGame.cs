using UnityEngine;

public class CardGame : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float angle = 7f;
    [SerializeField] private float gap = 10f;
    [SerializeField] private int cardsCount = 22;

    // create an array to store the cards
    private Card [] cards;

    void Start()
    {
        cards = new Card[cardsCount];
        GenerateDeck();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            int cardNumber = Random.Range(0,cardsCount);
            cards[cardNumber].SelectCard();
        }
    }

    private void GenerateDeck()
    {
        for (int i = 0; i < cardsCount; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            RectTransform cardRect = instance.GetComponent<RectTransform>();

            // This ranges symmetrically around zero for both odd and even decks.
            // For example, four cards use -1.5, -0.5, 0.5 and 1.5.
            float offsetFromCenter = i - ((cardsCount - 1) * 0.5f);

            Vector2 position = cardRect.anchoredPosition;
            position.x = offsetFromCenter * gap;
            cardRect.anchoredPosition = position;
            cardRect.localRotation = Quaternion.Euler(0f, 0f, -offsetFromCenter * angle);

            cards[i] = instance.GetComponent<Card>();
        }
    }
}
