using UnityEngine;
using FMODUnity;
using System.Collections;

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

public enum Location
    {
        Lobby = 0,
        Cafe = 1,
        Psychic = 2,
        Beach = 3
    }

public class MusicManager : MonoBehaviour
{

    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private StudioEventEmitter locationEmitter;
    [SerializeField] private StudioEventEmitter owlTalkEmitter, doeTalkEmitter, toadTalkEmitter;
    [SerializeField] private StudioEventEmitter wooshInEmitter, wooshOutEmitter;
    [SerializeField] private StudioEventEmitter clickEmitter;
    [SerializeField] private StudioEventEmitter cardFlipGoodEmitter, cardFlipBadEmitter, cardSlideEmitter;
    [SerializeField] private StudioEventEmitter deckFanEmitter, deckCollapseEmitter, testHoverEmitter;
    private WeirdFactor currentWeirdFactor;
    private Coroutine musicFadeCoroutine;

    void Start()
    {
        currentWeirdFactor = WeirdFactor.Weird0;
        musicEmitter.Play();
        locationEmitter.Play();
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

    public void PlayGoodCardFlipSFX()
    {
        cardFlipGoodEmitter.Play();
    }

    public void PlayBadCardFlipSFX()
    {
        cardFlipBadEmitter.Play();
    }

    public void PlayCardSlideSFX()
    {
        cardSlideEmitter.Play();
    }

    public void PlayDeckFanSFX()
    {
        deckFanEmitter.Play();
    }

    public void PlayDeckCollapseSFX()
    {
        deckCollapseEmitter.Play();
    }

    public void PlayTestHoverSFX()
    {
        testHoverEmitter.Play();
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

    public void SetLocation(Location location)
    {
        int requestedValue = (int)location;
        FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
        FMOD.RESULT setResult = studioSystem.setParameterByNameWithLabel("Location", location.ToString());
        FMOD.RESULT readResult = studioSystem.getParameterByName("Location", out float currentValue, out float finalValue);

        Debug.Log($"Location debug: requested {location} ({requestedValue}); set={setResult}, read={readResult}, current={currentValue}, final={finalValue}.");
    }

    public void FadeOutMusic(float duration)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    public void RestoreMusic()
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        bool allowFadeout = musicEmitter.AllowFadeout;
        musicEmitter.AllowFadeout = false;
        musicEmitter.Stop();
        musicEmitter.AllowFadeout = allowFadeout;
        musicEmitter.Play();

        FMOD.Studio.EventInstance musicInstance = musicEmitter.EventInstance;
        if (musicInstance.isValid())
        {
            musicInstance.setVolume(1f);
        }
    }

    private IEnumerator FadeOutMusicCoroutine(float duration)
    {
        FMOD.Studio.EventInstance musicInstance = musicEmitter.EventInstance;
        if (!musicInstance.isValid())
        {
            musicFadeCoroutine = null;
            yield break;
        }

        musicInstance.getVolume(out float startingVolume);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float volume = Mathf.Lerp(startingVolume, 0f, Mathf.Clamp01(elapsed / duration));
            musicInstance.setVolume(volume);
            yield return null;
        }

        musicInstance.setVolume(0f);
        musicEmitter.Stop();
        musicFadeCoroutine = null;
    }

}
