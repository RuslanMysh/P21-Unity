using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipiesDeliveredText;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
       if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();

            recipiesDeliveredText.text = DeliveryManager.Instance.GetSuccesfulRecipiesAmount().ToString();
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
