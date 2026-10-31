using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // ��� ������ � TMP_Dropdown
using YG;    // ��� ������ � PluginYG / YG2

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

        // ��������� ���������
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetVolume(savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // ��������� ������ �����
        if (languageDropdown != null)
        {
            // ������������� �� ��������� �������� � Dropdown
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            // ������������� ������� ������������ �������� � Dropdown �� ������ ������� ����������� YG2
            SetDropdownValueFromYG();
        }

        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        YandexAdsService.ShowStickyBanner(); // баннер на главном экране
    }

    private void OnEnable()
    {
        // ������������� �� ������� ����� ����� � YG2, ����� ���������� ������ ���������� ��� ������� ����� �����
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

    // ���������� ��� ������ ����� �� ����������� ������ (Dropdown)
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

    // ������������� ����������� �������� Dropdown � ������� ���������� YG2.lang
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