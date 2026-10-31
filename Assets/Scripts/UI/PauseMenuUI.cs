using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pause menu UI controller.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("Root")]
    [Tooltip("Panel root GameObject toggled on pause.")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    public static bool IsPaused { get; private set; }
    private bool _isPaused;

    #region Unity Lifecycle

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        GameInput.Instance.OnPauseAction += OnPauseRequested;

        Hide();
    }

    private void OnDestroy()
    {
        IsPaused = false;
        if (GameInput.Instance != null)
            GameInput.Instance.OnPauseAction -= OnPauseRequested;
    }

    #endregion

    #region Pause Logic

    private void OnPauseRequested(object sender, EventArgs e)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (KitchenGameManager.Instance.IsGameOver()) return;

        _isPaused = !_isPaused;

        if (_isPaused)
            Pause();
        else
            Resume();
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        MusicManager.Instance?.SetPaused(true);
        YandexAdsService.NotifyGameplayStop();
        Show();
    }

    public void Resume()
    {
        _isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        MusicManager.Instance?.SetPaused(false);

        if (KitchenGameManager.Instance.IsGamePlaying())
            YandexAdsService.NotifyGameplayStart();

        Hide();
    }

    private void GoToMainMenu()
    {
        if (mainMenuButton != null)
            mainMenuButton.interactable = false;

        Time.timeScale = 1f;
        IsPaused = false;
        MusicManager.Instance?.Stop();
        YandexAdsService.NotifyGameplayStop();
        YandexAdsService.ShowInterstitial(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    #endregion

    #region Visibility

    private void Show()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    private void Hide()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    #endregion
}
