using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

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

    public void SetPaused(bool paused)
    {
        if (audioSource == null) return;

        if (paused)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
