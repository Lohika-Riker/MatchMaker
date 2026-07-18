using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    bool raised = false;
    float distance = 50f;
    void Start()
    {
        
    }

    public void RaiseCard()
    {
        if (raised) {
            LowerCard();
            return;
        }
        
        Vector2 targetPosition = transform.position + (transform.up * distance);
        transform.DOMove(targetPosition, 0.2f);
        raised = true;
    }

    public void LowerCard()
    {
        if (!raised) {
            RaiseCard();
            return;
        }
        Vector2 targetPosition = transform.position + (-transform.up * distance);
        transform.DOMove(targetPosition, 0.2f);
        raised = false;
    }


}
