using DG.Tweening;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    float angle = 7f;
    float gap = 1;
    int cardsCount = 22;

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
        // spawn 22 cards rotated at an increasing offset
        float rotationOffset = cardsCount/2 * angle;
        float positionOffset = 200;
        for (int i = 0; i < cardsCount; i++)
        {
            GameObject instance = Instantiate(cardPrefab, this.transform);
            instance.transform.DORotate(new Vector3(0,0,rotationOffset), 0f);
            instance.transform.DOMoveX(positionOffset, 0f);
            rotationOffset -= angle;
            positionOffset += gap;

            cards[i] = instance.GetComponent<Card>();
        }
    }
}
