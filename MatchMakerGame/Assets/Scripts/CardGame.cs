// using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardGame : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float angle = 7f;
    [SerializeField] private float gap = 10f;
    [SerializeField] private int cardsCount = 22;

    // create an array to store the cards
    private Card [] cards;
    int cardNumber;
    private int cardCounter = 0;
    private Sequence fanSequence;

    void Start()
    {
        cards = new Card[cardsCount];
        cardNumber = Random.Range(0,cardsCount);
        GenerateDeck();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cardNumber = Random.Range(0,cardsCount);
            cards[cardNumber].SelectCard(cardCounter);
            cardCounter++;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cards[cardNumber].DiscardCard();
        }
    }

    private void GenerateDeck()
    {
        fanSequence = DOTween.Sequence();
        Vector2 prevPosition = new Vector2();
        // animate -> move from previous card position to current
        for (int i = 0; i < cardsCount; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            RectTransform cardRect = instance.GetComponent<RectTransform>();

            float offsetFromCenter = i - ((cardsCount - 1) * 0.5f);

            Vector2 position = cardRect.anchoredPosition;
            position.x = offsetFromCenter * gap;

            // use dotween to move card from prevPos to new pos
            
            cardRect.anchoredPosition = position;
            cardRect.localRotation = Quaternion.Euler(0f, 0f, -offsetFromCenter * angle);
            

            prevPosition = position;

            cards[i] = instance.GetComponent<Card>();
        }
    }
}
