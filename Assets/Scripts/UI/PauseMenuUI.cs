using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Меню паузы. Все UI-элементы настраиваются через Inspector.
/// Назначь этот компонент на корневой GameObject меню в сцене.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────
    [Header("Root")]
    [Tooltip("GameObject, который будет показан/скрыт при паузе. Обычно — сам этот GameObject или его дочерний контейнер.")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    // ── Состояние ─────────────────────────────────────────────────────────
    public static bool IsPaused { get; private set; }
    private bool _isPaused;

    // ─────────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void Start()
    {
        // Подписка на кнопки
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Подписка на ввод
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

    // ─────────────────────────────────────────────────────────────────────
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
        Time.timeScale = 1f;
        IsPaused = false;
        MusicManager.Instance?.Stop();
        YandexAdsService.ShowInterstitial();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────────
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
