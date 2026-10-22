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


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one GameManager!");
        }

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
        gamePlayingTimer = Mathf.Min(gamePlayingTimer + bonus, gamePlayingTimerMax + deliveryBonusTimeMax);
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
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    YandexAdsService.NotifyGameplayStart();
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public void ContinueWithExtraTime(float extraSeconds)
    {
        if (state != State.GameOver) return;

        gamePlayingTimer = Mathf.Max(extraSeconds, 1f);
        state = State.GamePlaying;
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
        float max = Mathf.Max(gamePlayingTimerMax, gamePlayingTimer);
        return gamePlayingTimer / max;
    }
}