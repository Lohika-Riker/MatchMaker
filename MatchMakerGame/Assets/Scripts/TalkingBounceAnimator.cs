using DG.Tweening;
using UnityEngine;

public class TalkingBounceAnimator : MonoBehaviour
{
    [SerializeField] private float bounceDistance = 10f;
    [SerializeField] private Vector2 rotationRange = new Vector2(3f, 7f);
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float resetDuration = 0.5f;

    private Sequence talkAnimation;
    private float restingLocalY;
    private Quaternion restingLocalRotation;
    private bool hasRestingPose;

    public void StartTalking()
    {
        KillTalkingAnimation();

        restingLocalY = transform.localPosition.y;
        restingLocalRotation = transform.localRotation;
        hasRestingPose = true;

        float randomRotation = Random.Range(rotationRange.x, rotationRange.y);
        float rotationAmount = Random.value > 0.5f ? randomRotation : -randomRotation;

        talkAnimation = DOTween.Sequence();
        talkAnimation.Append(transform.DOLocalRotate(new Vector3(0f, 0f, rotationAmount), rotationDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));

        talkAnimation.Insert(0f, transform.DOLocalMoveY(restingLocalY - bounceDistance, bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));
    }

    public void StopTalking()
    {
        StopTalking(false);
    }

    public void StopTalkingImmediately()
    {
        StopTalking(true);
    }

    private void StopTalking(bool instant)
    {
        KillTalkingAnimation();

        if (!hasRestingPose)
        {
            return;
        }

        if (instant)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.y = restingLocalY;
            transform.localPosition = localPosition;
            transform.localRotation = restingLocalRotation;
            return;
        }

        transform.DOLocalRotateQuaternion(restingLocalRotation, resetDuration);
        transform.DOLocalMoveY(restingLocalY, resetDuration);
    }

    private void KillTalkingAnimation()
    {
        if (talkAnimation != null)
        {
            talkAnimation.Kill();
            talkAnimation = null;
        }
    }

    private void OnDisable()
    {
        StopTalking(true);
    }
}
