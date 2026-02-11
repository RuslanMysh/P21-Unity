using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;

    private void Start()
    {
        plateKitchenObject.OnIgredientAdded += PlateKitchenObject_OnIgredientAdded;

        foreach (KitchenObjectSO_GameObject ingredient in kitchenObjectSOGameObjectList)
        {
            ingredient.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIgredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject ingredient in kitchenObjectSOGameObjectList)
        {
            if (ingredient.kitchenObjectSO == e.kitchenObjectSO)
            {
                ingredient.gameObject.SetActive(true);
            }
        }
    }  
}
