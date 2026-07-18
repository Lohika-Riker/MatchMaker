using UnityEngine;
using DG.Tweening;

public class FadeToBlack : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Tween fade;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        // DEBUG
        
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     Fade(true);
        // }
        // else if (Input.GetKeyDown(KeyCode.C))
        // {
        //     Fade(false);
        // }
    }

    public void Fade(bool fadeIn)
    {
        fade.Kill();
        float endValue;
        if (fadeIn)
        {
            endValue = 1f;
        }
        else
        {
            endValue = 0f;
        }

        fade = canvasGroup.DOFade(endValue, 2f);
    }
}
