using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;
    private float baseVolume;
    private bool isCooking;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource != null ? audioSource.volume : 0.2f;
    }

    private void Start()
    {
        if (stoveCounter != null)
        {
            stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        }

        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }

        ApplyVolume();
    }

    private void OnDestroy()
    {
        if (stoveCounter != null)
        {
            stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
        }

        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
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
        isCooking = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        UpdateSound();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        UpdateSound();
    }

    private void UpdateSound()
    {
        if (audioSource == null) return;

        bool isGamePlaying = KitchenGameManager.Instance != null && KitchenGameManager.Instance.IsGamePlaying();

        if (isCooking && isGamePlaying)
        {
            ApplyVolume();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
}
