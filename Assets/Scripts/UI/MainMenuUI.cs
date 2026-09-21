using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button setButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;

    private SettingsUI settingsUI;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });

        if (settingsPanel != null)
        {
            settingsUI = settingsPanel.GetComponent<SettingsUI>();
            if (settingsUI == null)
            {
                settingsUI = settingsPanel.AddComponent<SettingsUI>();
            }
        }

        setButton.onClick.AddListener(() =>
        {
            if (settingsUI != null)
            {
                settingsUI.Show();
            }
            else if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        });

        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(() =>
            {
                if (settingsUI != null)
                {
                    settingsUI.Hide();
                }
                else if (settingsPanel != null)
                {
                    settingsPanel.SetActive(false);
                }
            });
        }

        if (settingsUI != null)
        {
            settingsUI.Hide();
        }
        else if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
