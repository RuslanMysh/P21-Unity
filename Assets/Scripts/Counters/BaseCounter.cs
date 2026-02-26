using System;
using UnityEngine;
public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;

    private KitchenObject kitchenObject;
    [SerializeField] private Transform counterTopPoint;

    // virtual потому что мы хотим взаимодействовать не с базовой реализацией Interact,
    // а с какой-нибудь из дочерних
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact() была вызвана");
    }
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    public virtual void InteractAlternate(Player player)
    {
       
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

        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
}
