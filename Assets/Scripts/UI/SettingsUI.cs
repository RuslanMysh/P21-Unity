using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds a wood-styled settings panel at runtime to avoid heavy prefab deps.
/// Attach to an empty/overlay panel (existing SettingsPanel works).
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private bool buildContentIfMissing = true;

    private Slider musicSlider;
    private Slider soundSlider;
    private TextMeshProUGUI musicValueText;
    private TextMeshProUGUI soundValueText;
    private bool built;

    private static readonly Color WoodBg = new Color(0.45f, 0.28f, 0.12f, 0.96f);
    private static readonly Color PanelTint = new Color(0.86f, 0.58f, 0.09f, 0.22f);
    private static readonly Color LabelColor = new Color(0.95f, 0.90f, 0.78f, 1f);
    private static readonly Color TrackColor = new Color(0.25f, 0.16f, 0.08f, 1f);
    private static readonly Color FillColor = new Color(0.78f, 0.52f, 0.18f, 1f);

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        if (buildContentIfMissing)
        {
            EnsureBuilt();
        }

        Hide();
    }

    private void OnEnable()
    {
        EnsureBuilt();
        SyncFromSettings();
    }

    public void Show()
    {
        EnsureBuilt();
        SyncFromSettings();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SyncFromSettings()
    {
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
            UpdateValueLabel(musicValueText, GameSettings.MusicVolume);
        }

        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(GameSettings.SoundVolume);
            UpdateValueLabel(soundValueText, GameSettings.SoundVolume);
        }
    }

    private void EnsureBuilt()
    {
        if (built) return;
        if (transform.Find("SettingsWindow") != null)
        {
            BindExisting();
            built = true;
            return;
        }

        BuildPanel();
        built = true;
    }

    private void BindExisting()
    {
        musicSlider = transform.Find("SettingsWindow/MusicRow/Slider")?.GetComponent<Slider>();
        soundSlider = transform.Find("SettingsWindow/SoundRow/Slider")?.GetComponent<Slider>();
        musicValueText = transform.Find("SettingsWindow/MusicRow/Value")?.GetComponent<TextMeshProUGUI>();
        soundValueText = transform.Find("SettingsWindow/SoundRow/Value")?.GetComponent<TextMeshProUGUI>();
        WireSliders();
    }

    private void BuildPanel()
    {
        Image overlay = GetComponent<Image>();
        if (overlay != null)
        {
            overlay.color = PanelTint;
            overlay.raycastTarget = true;
        }

        RectTransform window = CreateUiObject("SettingsWindow", transform);
        Stretch(window, 0.5f, 0.5f, 0.5f, 0.5f, Vector2.zero, new Vector2(520f, 360f));
        Image windowImage = window.gameObject.AddComponent<Image>();
        windowImage.color = WoodBg;
        windowImage.raycastTarget = true;

        RectTransform titleRt = CreateUiObject("Title", window);
        Stretch(titleRt, 0.5f, 1f, 0.5f, 1f, new Vector2(0f, -36f), new Vector2(420f, 48f));
        TextMeshProUGUI title = titleRt.gameObject.AddComponent<TextMeshProUGUI>();
        ConfigureLabel(title, "НАСТРОЙКИ", 36f, FontStyles.Bold);

        musicSlider = CreateSliderRow(window, "MusicRow", "Музыка", new Vector2(0f, 40f), out musicValueText);
        soundSlider = CreateSliderRow(window, "SoundRow", "Звуки", new Vector2(0f, -50f), out soundValueText);

        if (closeButton == null)
        {
            closeButton = transform.Find("Close")?.GetComponent<Button>();
        }

        if (closeButton == null)
        {
            RectTransform closeRt = CreateUiObject("Close", window);
            Stretch(closeRt, 1f, 1f, 1f, 1f, new Vector2(-18f, -18f), new Vector2(72f, 72f));
            Image closeImg = closeRt.gameObject.AddComponent<Image>();
            closeImg.color = new Color(0.85f, 0.25f, 0.2f, 1f);
            closeButton = closeRt.gameObject.AddComponent<Button>();
            closeButton.targetGraphic = closeImg;

            RectTransform closeTextRt = CreateUiObject("Text", closeRt);
            StretchFull(closeTextRt);
            TextMeshProUGUI closeText = closeTextRt.gameObject.AddComponent<TextMeshProUGUI>();
            ConfigureLabel(closeText, "X", 28f, FontStyles.Bold);
        }

        closeButton.onClick.RemoveListener(Hide);
        closeButton.onClick.AddListener(Hide);

        WireSliders();
    }

    private Slider CreateSliderRow(Transform parent, string name, string label, Vector2 anchoredPos, out TextMeshProUGUI valueText)
    {
        RectTransform row = CreateUiObject(name, parent);
        Stretch(row, 0.5f, 0.5f, 0.5f, 0.5f, anchoredPos, new Vector2(440f, 70f));

        RectTransform labelRt = CreateUiObject("Label", row);
        Stretch(labelRt, 0f, 0.5f, 0f, 0.5f, new Vector2(10f, 16f), new Vector2(180f, 36f));
        TextMeshProUGUI labelTmp = labelRt.gameObject.AddComponent<TextMeshProUGUI>();
        ConfigureLabel(labelTmp, label, 26f, FontStyles.Bold);
        labelTmp.alignment = TextAlignmentOptions.Left;

        RectTransform valueRt = CreateUiObject("Value", row);
        Stretch(valueRt, 1f, 0.5f, 1f, 0.5f, new Vector2(-10f, 16f), new Vector2(70f, 36f));
        valueText = valueRt.gameObject.AddComponent<TextMeshProUGUI>();
        ConfigureLabel(valueText, "100%", 22f, FontStyles.Normal);
        valueText.alignment = TextAlignmentOptions.Right;

        RectTransform sliderRt = CreateUiObject("Slider", row);
        Stretch(sliderRt, 0.5f, 0.5f, 0.5f, 0.5f, new Vector2(0f, -14f), new Vector2(400f, 28f));
        Slider slider = sliderRt.gameObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        RectTransform bgRt = CreateUiObject("Background", sliderRt);
        StretchFull(bgRt);
        Image bg = bgRt.gameObject.AddComponent<Image>();
        bg.color = TrackColor;

        RectTransform fillArea = CreateUiObject("Fill Area", sliderRt);
        StretchFull(fillArea);
        RectTransform fillRt = CreateUiObject("Fill", fillArea);
        StretchFull(fillRt);
        Image fill = fillRt.gameObject.AddComponent<Image>();
        fill.color = FillColor;

        RectTransform handleArea = CreateUiObject("Handle Slide Area", sliderRt);
        StretchFull(handleArea);
        RectTransform handleRt = CreateUiObject("Handle", handleArea);
        handleRt.sizeDelta = new Vector2(28f, 28f);
        Image handle = handleRt.gameObject.AddComponent<Image>();
        handle.color = LabelColor;

        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private void WireSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(v =>
            {
                GameSettings.MusicVolume = v;
                UpdateValueLabel(musicValueText, v);
            });
        }

        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveAllListeners();
            soundSlider.onValueChanged.AddListener(v =>
            {
                GameSettings.SoundVolume = v;
                UpdateValueLabel(soundValueText, v);
            });
        }
    }

    private static void UpdateValueLabel(TextMeshProUGUI label, float value)
    {
        if (label != null)
        {
            label.text = Mathf.RoundToInt(value * 100f) + "%";
        }
    }

    private static void ConfigureLabel(TextMeshProUGUI tmp, string text, float size, FontStyles style)
    {
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = LabelColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
    }

    private static RectTransform CreateUiObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private static void Stretch(RectTransform rt, float minX, float minY, float maxX, float maxY, Vector2 anchoredPos, Vector2 size)
    {
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

}
