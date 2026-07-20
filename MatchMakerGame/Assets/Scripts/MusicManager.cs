using UnityEngine;
using FMODUnity;

public enum WeirdFactor
    {
        Weird0 = 0,
        Weird1 = 1,
        Weird2 = 2,
        Weird3 = 3,
        Weird4 = 4,
        Weird5 = 5,
        Weird6 = 6,
        End = 7
    }
public class MusicManager : MonoBehaviour
{

    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private StudioEventEmitter owlTalkEmitter, doeTalkEmitter, toadTalkEmitter;
    [SerializeField] private StudioEventEmitter wooshInEmitter, wooshOutEmitter;
    [SerializeField] private StudioEventEmitter clickEmitter;
    private WeirdFactor currentWeirdFactor;

    void Start()
    {
        currentWeirdFactor = WeirdFactor.Weird0;
        musicEmitter.Play();
    }

    void Update()
    {
        // DEBUG STUFF
        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     IncreaseWeirdFactor();
        // }
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     StartToadTalk();
        // }
        // else if (Input.GetKeyDown(KeyCode.P))
        // {
        //     StopToadTalk();
        // }
    }

    public void PlayClickSFX()
    {
        clickEmitter.Play();
    }

    public void PlayCharacterMoveInSFX()
    {
        wooshInEmitter.Play();
    }

     public void PlayCharacterMoveOutSFX()
    {
        wooshOutEmitter.Play();
    }

    public void StartOwlTalk()
    {
        owlTalkEmitter.Play();
    }
    public void StopOwlTalk()
    {
        owlTalkEmitter.Stop();
    }

    public void StartDoeTalk()
    {
        doeTalkEmitter.Play();
    }

    public void StopDoeTalk()
    {
        doeTalkEmitter.Stop();
    }

    public void StartToadTalk()
    {
        toadTalkEmitter.Play();
    }

    public void StopToadTalk()
    {
        toadTalkEmitter.Stop();
    }

    public void IncreaseWeirdFactor()
    {
        if (currentWeirdFactor < WeirdFactor.End)
        {
            SetWeirdFactor((int)currentWeirdFactor + 1);
        }
    }

    public void SetWeirdFactor(int weirdFactor)
    {
        int clampedWeirdFactor = Mathf.Clamp(weirdFactor, (int)WeirdFactor.Weird0, (int)WeirdFactor.End);
        currentWeirdFactor = (WeirdFactor)clampedWeirdFactor;
        print($"Setting weird factor to {currentWeirdFactor}");
        musicEmitter.SetParameter("Weird factor", clampedWeirdFactor);
    }

}
