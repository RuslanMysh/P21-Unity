using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private SettingsUI settingsUI;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            if (settingsUI != null && settingsUI.gameObject.activeSelf)
            {
                settingsUI.Hide();
            }

            KitchenGameManager.Instance.TogglePauseGame();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            YandexAdsService.ShowInterstitial();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        EnsureSettingsUi();

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() =>
            {
                settingsUI?.Show();
            });
        }
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        KitchenGameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        Hide();
    }

    private void EnsureSettingsUi()
    {
        if (settingsUI != null)
        {
            return;
        }

        Transform existing = transform.Find("SettingsPanel");
        GameObject panelGo;
        if (existing != null)
        {
            panelGo = existing.gameObject;
        }
        else
        {
            panelGo = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGo.layer = 5;
            RectTransform rt = panelGo.GetComponent<RectTransform>();
            rt.SetParent(transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Image img = panelGo.GetComponent<Image>();
            img.color = new Color(0.86f, 0.58f, 0.09f, 0.22f);
        }

        settingsUI = panelGo.GetComponent<SettingsUI>();
        if (settingsUI == null)
        {
            settingsUI = panelGo.AddComponent<SettingsUI>();
        }

        if (settingsButton == null)
        {
            settingsButton = CreatePauseButton("SettingsButton", "\u041D\u0410\u0421\u0422\u0420\u041E\u0419\u041A\u0418", new Vector2(0f, 180f));
        }
    }

    private Button CreatePauseButton(string name, string label, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = 5;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        rt.sizeDelta = new Vector2(160f, 30f);
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

        TMPro.TextMeshProUGUI tmp = textGo.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontSize = 16f;
        tmp.fontStyle = TMPro.FontStyles.Bold;
        tmp.color = new Color(0.95f, 0.90f, 0.78f, 1f);
        tmp.raycastTarget = false;

        return button;
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        settingsUI?.Hide();
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        Show();
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
