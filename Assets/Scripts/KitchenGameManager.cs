using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 90f;
    [SerializeField] private float deliveryBonusTime = 4f;
    [SerializeField] private float deliveryBonusTimeMax = 12f;


    private float currentGamePlayingTimerMax;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one GameManager!");
        }

        currentGamePlayingTimerMax = gamePlayingTimerMax;
        state = State.WaitingToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        if (!IsGamePlaying()) return;

        float bonus = Mathf.Min(deliveryBonusTime, deliveryBonusTimeMax - 0.01f);
        gamePlayingTimer = Mathf.Min(gamePlayingTimer + bonus, currentGamePlayingTimerMax + deliveryBonusTimeMax);
        if (gamePlayingTimer > currentGamePlayingTimerMax)
        {
            currentGamePlayingTimerMax = gamePlayingTimer;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    currentGamePlayingTimerMax = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    YandexAdsService.HideStickyBanner(); // скрываем баннер во время игры
                    YandexAdsService.NotifyGameplayStart();
                }
                break;
            case State.GamePlaying:
                if (!PauseMenuUI.IsPaused && Time.timeScale == 0f)
                {
                    Time.timeScale = 1f;
                }
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    Time.timeScale = 0f;
                    YandexAdsService.ShowInterstitial(); // interstitial на логичной паузе (конец раунда)
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    YandexAdsService.ShowStickyBanner(); // показываем баннер на экране Game Over
                }
                break;
            case State.GameOver:
                break;
        }
    }

    /// <summary>
    /// Добавляет секунды к таймеру во время игры (state == GamePlaying).
    /// </summary>
    public void AddTime(float extraSeconds)
    {
        if (state != State.GamePlaying) return;

        gamePlayingTimer += extraSeconds;
        if (gamePlayingTimer > currentGamePlayingTimerMax)
        {
            currentGamePlayingTimerMax = gamePlayingTimer;
        }
    }

    /// <summary>
    /// Продолжение игры после GameOver с новым таймером.
    /// </summary>
    public void ContinueWithExtraTime(float extraSeconds)
    {
        if (state != State.GameOver) return;

        gamePlayingTimer = Mathf.Max(extraSeconds, 1f);
        currentGamePlayingTimerMax = gamePlayingTimer;
        state = State.GamePlaying;
        Time.timeScale = 1f;

#if PLUGIN_YG_2
        if (YG.PauseGameYG.inst != null)
        {
            YG.PauseGameYG.SetState(1f, false, true);
        }
        YG.YG2.PauseGame(false);
#endif

        Time.timeScale = 1f;

        OnStateChanged?.Invoke(this, EventArgs.Empty);
        YandexAdsService.NotifyGameplayStart();
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetGameplayingTimerNormalized()
    {
        if (currentGamePlayingTimerMax <= 0f) return 0f;
        return Mathf.Clamp01(gamePlayingTimer / currentGamePlayingTimerMax);
    }
}