using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private IHasProgress hasProgress;
    [SerializeField] private GameObject hasProgressGameObject;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError("GameObject " + hasProgressGameObject + " не имеет компонента, " +
                "который имплементирует IHasProgress");
        }    

        hasProgress.OnProgressChanged += CuttingCounter_OnProgressChanged;
        Hide();
    }

    private void CuttingCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1f)
        {
            Hide();
        }
        else
        {
            Show();
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
