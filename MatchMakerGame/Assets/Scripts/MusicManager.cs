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

    [SerializeField] private FMODUnity.StudioEventEmitter musicEmitter;
    private WeirdFactor currentWeirdFactor;

    void Start()
    {
        currentWeirdFactor = WeirdFactor.Weird0;
        musicEmitter.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            IncreaseWeirdFactor();
        }
    }

    public void IncreaseWeirdFactor()
    {
        if (currentWeirdFactor < WeirdFactor.End)
        {
            currentWeirdFactor++;
            SetWeirdFactor(currentWeirdFactor);
        }
    }

    private void SetWeirdFactor(WeirdFactor weirdFactor)
    {  
        print($"Setting weird factor to {weirdFactor}");
        musicEmitter.SetParameter("Weird factor", (float)weirdFactor);
    }

}
