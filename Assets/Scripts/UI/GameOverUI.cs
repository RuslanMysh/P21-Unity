using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipiesDeliveredText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button extraTimeButton;

    private bool extraTimeUsed;
    private TextMeshProUGUI extraTimeLabel;

    private void Awake()
    {
        EnsureButtons();
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void EnsureButtons()
    {
        if (mainMenuButton == null)
        {
            mainMenuButton = CreateButton("MainMenuButton", "\u0412 \u041C\u0415\u041D\u042E", new Vector2(0f, -90f));
        }

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(() =>
        {
            YandexAdsService.ShowInterstitial();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        if (extraTimeButton == null)
        {
            extraTimeButton = CreateButton("ExtraTimeButton", "+\u0412\u0420\u0415\u041C\u042F (\u0420\u0415\u041A\u041B\u0410\u041C\u0410)", new Vector2(0f, -170f));
        }

        extraTimeLabel = extraTimeButton.GetComponentInChildren<TextMeshProUGUI>();
        extraTimeButton.onClick.RemoveAllListeners();
        extraTimeButton.onClick.AddListener(() =>
        {
            if (extraTimeUsed) return;

            YandexAdsService.ShowRewarded(YandexAdsService.ExtraTimeRewardId, () =>
            {
                extraTimeUsed = true;
                KitchenGameManager.Instance.ContinueWithExtraTime(25f);
                if (extraTimeLabel != null)
                {
                    extraTimeLabel.text = "\u0418\u0421\u041F\u041E\u041B\u042C\u0417\u041E\u0412\u0410\u041D\u041E";
                }

                extraTimeButton.interactable = false;
                Hide();
            });
        });
    }

    private Button CreateButton(string name, string label, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = 5;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(320f, 70f);
        rt.anchoredPosition = anchoredPos;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.55f, 0.35f, 0.16f, 1f);

        Button button = go.GetComponent<Button>();
        button.targetGraphic = img;

        GameObject textGo = new GameObject("Text", typeof(RectTransform));
        textGo.layer = 5;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.SetParent(rt, false);
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 26f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(0.95f, 0.90f, 0.78f, 1f);
        tmp.raycastTarget = false;

        return button;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            if (extraTimeButton != null)
            {
                extraTimeButton.gameObject.SetActive(!extraTimeUsed);
                extraTimeButton.interactable = !extraTimeUsed;
                if (extraTimeLabel != null && !extraTimeUsed)
                {
                    extraTimeLabel.text = "+\u0412\u0420\u0415\u041C\u042F (\u0420\u0415\u041A\u041B\u0410\u041C\u0410)";
                }
            }

            Show();
            recipiesDeliveredText.text = DeliveryManager.Instance.GetSuccesfulRecipiesAmount().ToString();
            YandexAdsService.NotifyGameplayStop();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
