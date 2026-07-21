using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Min(1f)] private float hoverScale = 1.05f;
    [SerializeField, Min(0f)] private float pressedScale = 0.98f;
    [SerializeField, Min(0f)] private float animationDuration = 0.15f;
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Vector3 originalScale;
    private Tween scaleTween;
    private bool isHovered;
    private bool isPressed;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        AnimateTo(originalScale * (isPressed ? pressedScale : hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        AnimateTo(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        AnimateTo(originalScale * pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        AnimateTo(isHovered ? originalScale * hoverScale : originalScale);
    }

    private void AnimateTo(Vector3 targetScale)
    {
        scaleTween?.Kill();
        scaleTween = transform
            .DOScale(targetScale, animationDuration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        scaleTween?.Kill();
        scaleTween = null;
        isHovered = false;
        isPressed = false;
        transform.localScale = originalScale;
    }
}
