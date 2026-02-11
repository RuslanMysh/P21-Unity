using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
       if (!HasKitchenObject())
        {
            // на тумбе ничего не лежит
            if (player.HasKitchenObject())
            {
                // у игрока есть в руках объект на тумбочку
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
       else
        {  
            // на тумбочке есть объект
            if (player.HasKitchenObject())
            {
                // у игрока уже есть объект в руках и это тарелка
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // игрок держит тарелку
                   if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    // игрок держит объект, но не тарелку
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // на тумбочке есть тарелка
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // у игрока ничего нет в руках
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
