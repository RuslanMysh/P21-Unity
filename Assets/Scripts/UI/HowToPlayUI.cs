using UnityEngine;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [Header("UI элементы")]
    [Tooltip("Канвас или панель с текстом 'Как играть'")]
    [SerializeField] private GameObject howToPlayPanel;

    [Tooltip("Кнопка открытия (можно не назначать, если используешь OnClick в инспекторе)")]
    [SerializeField] private Button openButton;

    [Tooltip("Кнопка закрытия (крестик или 'Назад')")]
    [SerializeField] private Button closeButton;

    [Header("Дополнительно")]
    [Tooltip("Закрывать панель по нажатию клавиши Esc")]
    [SerializeField] private bool closeOnEscape = true;

    private void Awake()
    {
        // Панель скрыта при старте
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // Подписка на кнопку открытия
        if (openButton != null)
        {
            openButton.onClick.AddListener(Open);
        }

        // Подписка на кнопку закрытия
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    private void Update()
    {
        if (closeOnEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            if (howToPlayPanel != null && howToPlayPanel.activeSelf)
            {
                Close();
            }
        }
    }

    public void Open()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public void Close()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    public void Toggle()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(!howToPlayPanel.activeSelf);
        }
    }
}
