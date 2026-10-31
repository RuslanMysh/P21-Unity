using UnityEngine;
#if PLUGIN_YG_2
using YG;
#endif

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private bool isWindowFocused = true;
    private bool isManuallyPaused = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        ApplyVolume();
    }

    private void OnEnable()
    {
#if PLUGIN_YG_2
        YG2.onFocusWindowGame += OnFocusChanged;
        YG2.onCloseInterAdv += OnAdClosed;
        YG2.onCloseRewardedAdv += OnAdClosed;
#endif
    }

    private void OnDisable()
    {
#if PLUGIN_YG_2
        YG2.onFocusWindowGame -= OnFocusChanged;
        YG2.onCloseInterAdv -= OnAdClosed;
        YG2.onCloseRewardedAdv -= OnAdClosed;
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        OnFocusChanged(hasFocus);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        OnFocusChanged(!pauseStatus);
    }

    private void OnFocusChanged(bool hasFocus)
    {
        if (isWindowFocused == hasFocus) return;
        isWindowFocused = hasFocus;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (!hasFocus)
        {
            // Focus lost: mute all game audio (Yandex requirement §1)
            AudioListener.pause = true;
            if (audioSource != null)
            {
                audioSource.Pause();
            }

            if (KitchenGameManager.Instance != null && KitchenGameManager.Instance.IsGamePlaying() && !PauseMenuUI.IsPaused)
            {
                YandexAdsService.NotifyGameplayStop();
            }
        }
        else
        {
            // Focus regained: do not unpause if ad is still playing
#if PLUGIN_YG_2
            if (YG2.nowAdsShow)
            {
                return;
            }
#endif
            AudioListener.pause = false;

            if (!isManuallyPaused && !PauseMenuUI.IsPaused)
            {
                if (audioSource != null)
                {
                    audioSource.UnPause();
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }

            if (KitchenGameManager.Instance != null && KitchenGameManager.Instance.IsGamePlaying() && !PauseMenuUI.IsPaused)
            {
                YandexAdsService.NotifyGameplayStart();
            }
        }
    }

    private void OnAdClosed()
    {
        if (isWindowFocused)
        {
            AudioListener.pause = false;

            if (!isManuallyPaused && !PauseMenuUI.IsPaused)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.UnPause();
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void ApplyVolume()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.volume = GameSettings.MusicVolume;
        }
    }

    public void Stop()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }

    public void SetPaused(bool paused)
    {
        isManuallyPaused = paused;
        if (audioSource == null) return;

        if (paused)
        {
            audioSource.Pause();
        }
        else
        {
            if (isWindowFocused)
            {
                audioSource.UnPause();
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }
    }
}
