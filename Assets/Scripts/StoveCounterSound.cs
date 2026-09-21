using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;

    private float baseVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource != null ? audioSource.volume : 0.2f;
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        ApplyVolume();
    }

    private void Update()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            ApplyVolume();
        }
    }

    private void ApplyVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = baseVolume * GameSettings.SoundVolume;
        }
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;

        if (playSound)
        {
            ApplyVolume();
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
