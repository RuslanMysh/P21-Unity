using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button setButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;
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

        settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        
    }
}
