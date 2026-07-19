using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RorschachTest : MonoBehaviour
{
    [SerializeField] private Sprite rorschach1,rorschach2, rorschach3; 
    [SerializeField] private Image testImage;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private KeyCode showKey = KeyCode.S;
    [SerializeField] private KeyCode hideKey = KeyCode.H;
    [SerializeField] private KeyCode swapImageKey = KeyCode.Space;
    [SerializeField] private float pulseScale = 1.03f;
    [SerializeField] private float pulseDuration = 1.25f;
    [SerializeField] private float rotationAngle = 2f;
    [SerializeField] private float rotationDuration = 2.5f;

    private int testCounter;
    private RectTransform rectTransform;
    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;
    private Vector3 restingScale;
    private Vector3 restingRotation;

    void Awake()
    {
        testCounter = 0;
        rectTransform = (RectTransform)transform;
        visiblePosition = rectTransform.anchoredPosition;
        restingScale = rectTransform.localScale;
        restingRotation = rectTransform.localEulerAngles;

        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas != null ? canvas.rootCanvas.transform as RectTransform : null;
        float slideDistance = canvasRect != null ? canvasRect.rect.height : Screen.height;
        slideDistance += rectTransform.rect.height;

        hiddenPosition = visiblePosition + Vector2.down * slideDistance;
        rectTransform.anchoredPosition = hiddenPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(showKey))
        {
            Show();
        }

        if (Input.GetKeyDown(hideKey))
        {
            Hide();
        }

        if (Input.GetKeyDown(swapImageKey))
        {
            SwapImage();
        }
    }

    public void Hide()
    {
        StopParentAnimation();
        rectTransform.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack);
    }

    public void Show()
    {
        StartParentAnimation();
        rectTransform.DOAnchorPos(visiblePosition, slideDuration).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        rectTransform?.DOKill();
    }

    private void StartParentAnimation()
    {
        StopParentAnimation();

        rectTransform.DOScale(restingScale * pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        Vector3 rotatedAngle = restingRotation + new Vector3(0f, 0f, -rotationAngle);
        rectTransform.DOLocalRotate(rotatedAngle, rotationDuration, RotateMode.Fast)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopParentAnimation()
    {
        rectTransform.DOKill();
        rectTransform.localScale = restingScale;
        rectTransform.localEulerAngles = restingRotation;
    }

    public void SwapImage()
    {
        testCounter = testCounter % 3 + 1;
        SetImage(testCounter);
    }

    public void ShowTest(int testNumber)
    {
        SetImage(testNumber);
        Show();
    }

    private void SetImage(int testNumber)
    {
        if (testNumber >= 1 && testNumber <= 3)
        {
            testCounter = testNumber;
        }

        if (testNumber == 1)
        {
            testImage.sprite = rorschach1;
        }
        else if (testNumber == 2)
        {
            testImage.sprite = rorschach2;
        }
        else if (testNumber == 3)
        {
            testImage.sprite = rorschach3;
        }
        else
        {
            Debug.LogWarning($"Rorschach test {testNumber} does not exist.");
        }
    }
}
