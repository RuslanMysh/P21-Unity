using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Для работы с TMP_Dropdown
using YG;    // Для работы с PluginYG / YG2

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
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });

        setButton.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(true);
        });

        closeSettingsButton.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(false);
        });

        // Настройка громкости
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetVolume(savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Настройка выбора языка
        if (languageDropdown != null)
        {
            // Подписываемся на изменение значения в Dropdown
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            // Устанавливаем текущее отображаемое значение в Dropdown на основе текущей локализации YG2
            SetDropdownValueFromYG();
        }

        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        // Подписываемся на событие смены языка в YG2, чтобы выпадающий список обновлялся при внешней смене языка
        YG2.onSwitchLang += UpdateDropdownOnLangChange;
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= UpdateDropdownOnLangChange;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    // Вызывается при выборе опции из выпадающего списка (Dropdown)
    private void OnLanguageChanged(int index)
    {
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
    }

    // Синхронизация визуального значения Dropdown с текущей настройкой YG2.lang
    private void SetDropdownValueFromYG()
    {
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
    }

    private void UpdateDropdownOnLangChange(string lang)
    {
        SetDropdownValueFromYG();
    }
}