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
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        SetupButtons();
        Hide();
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
                QuitButton.interactable = false;
                Time.timeScale = 1f;
                YandexAdsService.NotifyGameplayStop();
                YandexAdsService.ShowInterstitial(() =>
                {
                    Loader.Load(Loader.Scene.MainMenuScene);
                });
            });
        }
        else
        {
            Debug.LogWarning("GameOverUI: QuitButton is not assigned in Inspector!");
        }

        if (AdvButton != null)
        {
            AdvButton.onClick.RemoveAllListeners();
            AdvButton.onClick.AddListener(() =>
            {
                AdvButton.interactable = false;
                YandexAdsService.ShowRewarded(
                    YandexAdsService.ExtraTimeRewardId,
                    onRewarded: () =>
                    {
                        KitchenGameManager.Instance.ContinueWithExtraTime(25f);
                        Hide();
                        Time.timeScale = 1f;
                    },
                    onFailed: () =>
                    {
                        if (AdvButton != null)
                            AdvButton.interactable = true;
                    }
                );
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
        if (AdvButton != null)
            AdvButton.interactable = true;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        YandexAdsService.HideStickyBanner(); // Hide sticky banner when leaving game over
        Time.timeScale = 1f;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
