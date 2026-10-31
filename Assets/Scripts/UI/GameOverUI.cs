using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipiesDeliveredText;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button AdvButton;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Используем CanvasGroup для скрытия, чтобы объект оставался активным
        // и подписка на OnStateChanged не терялась
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        SetupButtons();
        Hide(); // Скрываем сразу в Awake, до первого кадра
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void OnDestroy()
    {
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;

        Time.timeScale = 1f;
    }


    private void SetupButtons()
    {
        if (QuitButton != null)
        {
            QuitButton.onClick.RemoveAllListeners();
            QuitButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                YandexAdsService.ShowInterstitial();
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        }
        else
        {
            Debug.LogWarning("GameOverUI: QuitButton не назначена в Inspector!");
        }

        if (AdvButton != null)
        {
            AdvButton.onClick.RemoveAllListeners();
            AdvButton.onClick.AddListener(() =>
            {
                AdvButton.interactable = false;
                YandexAdsService.ShowRewarded(YandexAdsService.ExtraTimeRewardId, () =>
                {
                    KitchenGameManager.Instance.ContinueWithExtraTime(25f);
                    Hide();
                    Time.timeScale = 1f;
                });
            });
        }
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();

            if (recipiesDeliveredText != null)
                recipiesDeliveredText.text = DeliveryManager.Instance.GetSuccesfulRecipiesAmount().ToString();
            else
                Debug.LogWarning("GameOverUI: recipiesDeliveredText is not assigned in Inspector!");

            YandexAdsService.NotifyGameplayStop();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        Time.timeScale = 0f;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        YandexAdsService.HideStickyBanner(); // скрываем баннер при продолжении игры
        Time.timeScale = 1f;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
