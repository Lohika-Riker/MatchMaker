using UnityEngine;

public class Fly : MonoBehaviour
{
    [Header("Hover")]
    [SerializeField] private Vector2 circleRadius = new Vector2(10f, 6f);
    [SerializeField] private float circleSpeed = 2.5f;

    [Header("Erratic movement")]
    [SerializeField] private Vector2 dartDistance = new Vector2(14f, 9f);
    [SerializeField] private Vector2 timeBetweenDarts = new Vector2(0.8f, 2.2f);
    [SerializeField] private Vector2 dartDuration = new Vector2(0.12f, 0.28f);
    [SerializeField] private float movementSmoothing = 16f;

    private RectTransform rectTransform;
    private Vector2 hoverCenter;
    private Vector2 currentOffset;
    private Vector2 dartOffset;
    private Vector2 dartStart;
    private Vector2 dartTarget;
    private float circleAngle;
    private float dartTimer;
    private float currentDartDuration;
    private float nextDartTimer;
    private bool isDarting;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        hoverCenter = rectTransform != null
            ? rectTransform.anchoredPosition
            : (Vector2)transform.localPosition;

        circleAngle = Random.Range(0f, Mathf.PI * 2f);
        ScheduleNextDart();
    }

    public void SetHoverCenter(Vector2 position)
    {
        hoverCenter = position;
        currentOffset = Vector2.zero;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
        else
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x = position.x;
            localPosition.y = position.y;
            transform.localPosition = localPosition;
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        circleAngle += circleSpeed * deltaTime;

        if (isDarting)
        {
            dartTimer += deltaTime;
            float progress = Mathf.Clamp01(dartTimer / currentDartDuration);

            // A quick outward flick followed by a softer return to the orbit.
            float outAndBack = Mathf.Sin(progress * Mathf.PI);
            dartOffset = Vector2.LerpUnclamped(dartStart, dartTarget, outAndBack);

            if (progress >= 1f)
            {
                isDarting = false;
                dartOffset = Vector2.zero;
                ScheduleNextDart();
            }
        }
        else
        {
            nextDartTimer -= deltaTime;
            if (nextDartTimer <= 0f)
            {
                BeginDart();
            }
        }

        Vector2 circleOffset = new Vector2(
            Mathf.Cos(circleAngle) * circleRadius.x,
            Mathf.Sin(circleAngle) * circleRadius.y);
        Vector2 targetOffset = circleOffset + dartOffset;
        float smoothing = 1f - Mathf.Exp(-movementSmoothing * deltaTime);
        currentOffset = Vector2.Lerp(currentOffset, targetOffset, smoothing);

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = hoverCenter + currentOffset;
        }
        else
        {
            Vector3 position = transform.localPosition;
            position.x = hoverCenter.x + currentOffset.x;
            position.y = hoverCenter.y + currentOffset.y;
            transform.localPosition = position;
        }
    }

    private void BeginDart()
    {
        isDarting = true;
        dartTimer = 0f;
        currentDartDuration = Random.Range(dartDuration.x, dartDuration.y);
        dartStart = dartOffset;
        dartTarget = new Vector2(
            Random.Range(-dartDistance.x, dartDistance.x),
            Random.Range(-dartDistance.y, dartDistance.y));
    }

    private void ScheduleNextDart()
    {
        nextDartTimer = Random.Range(timeBetweenDarts.x, timeBetweenDarts.y);
    }
}
