using UnityEngine;

/// <summary>
/// Lightweight PlayerPrefs-backed audio settings for WebGL.
/// </summary>
public static class GameSettings
{
    private const string MusicVolumeKey = "settings_music_volume";
    private const string SoundVolumeKey = "settings_sound_volume";

    private const float DefaultMusicVolume = 0.3f;
    private const float DefaultSoundVolume = 1f;

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
        set
        {
            float clamped = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, clamped);
            PlayerPrefs.Save();
            MusicManager.Instance?.ApplyVolume();
        }
    }

    public static float SoundVolume
    {
        get => PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
        set
        {
            float clamped = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SoundVolumeKey, clamped);
            PlayerPrefs.Save();
        }
    }
}
