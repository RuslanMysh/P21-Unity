using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    // TODO:
    // 4. добавить анимацию

    private KitchenObject kitchenObject;
    [SerializeField] private Transform counterTopPoint;

    // virtual потому что мы хотим взаимодействовать не с базовой реализацией Interact,
    // а с какой-нибудь из дочерних
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact() была вызвана");
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }
}
