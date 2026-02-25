using UnityEngine;
using UnityEngine.UI;

public class GameOverClockUI : MonoBehaviour
{
    [SerializeField] private Image timeImage;

    private void Update()
    {
        timeImage.fillAmount = KitchenGameManager.Instance.GetGameplayingTimerNormalized();
    }
}
