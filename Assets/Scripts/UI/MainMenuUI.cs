using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if PLUGIN_YG_2
using YG;
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button setButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;

    [Header("Audio Settings")]
    [SerializeField] private Slider volumeSlider;

    [Header("Localization Settings")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    private const string VOLUME_KEY = "MasterVolume";

    private void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(() =>
            {
                playButton.interactable = false;
                Loader.Load(Loader.Scene.GameScene);
            });
        }

        if (setButton != null)
        {
            setButton.onClick.AddListener(() =>
            {
                if (settingsPanel != null)
                    settingsPanel.SetActive(true);
            });
        }

        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(() =>
            {
                if (settingsPanel != null)
                    settingsPanel.SetActive(false);
            });
        }

        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetVolume(savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            SetDropdownValueFromYG();
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        YandexAdsService.ShowStickyBanner();
    }

    private void Start()
    {
        SetDropdownValueFromYG();
    }

    private void OnEnable()
    {
#if PLUGIN_YG_2
        YG2.onSwitchLang += UpdateDropdownOnLangChange;
        YG2.onGetSDKData += SetDropdownValueFromYG;
#endif
    }

    private void OnDisable()
    {
#if PLUGIN_YG_2
        YG2.onSwitchLang -= UpdateDropdownOnLangChange;
        YG2.onGetSDKData -= SetDropdownValueFromYG;
#endif
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    private void OnLanguageChanged(int index)
    {
#if PLUGIN_YG_2
        switch (index)
        {
            case 0:
                YG2.SwitchLanguage("ru");
                break;
            case 1:
                YG2.SwitchLanguage("en");
                break;
            case 2:
                YG2.SwitchLanguage("tr");
                break;
        }
#endif
    }

    private void SetDropdownValueFromYG()
    {
        if (languageDropdown == null) return;

#if PLUGIN_YG_2
        switch (YG2.lang)
        {
            case "ru":
                languageDropdown.SetValueWithoutNotify(0);
                break;
            case "en":
                languageDropdown.SetValueWithoutNotify(1);
                break;
            case "tr":
                languageDropdown.SetValueWithoutNotify(2);
                break;
        }
#endif
    }

    private void UpdateDropdownOnLangChange(string lang)
    {
        SetDropdownValueFromYG();
    }
}
