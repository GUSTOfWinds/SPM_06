using ItemNamespace;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using Inventory_scripts;
using UnityEngine.InputSystem;

public class PlayerItemUsageController : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    public ItemBase itemBase; // Will need to be updated if another item is being used.

    [SerializeField] private GameObject heldItemWorldObject;
    [SerializeField] private GameObject holdingHand;

    private Type currentActingComponentType;
    private ItemBaseBehaviour currentActingComponent;


    public void Start()
    {
        ChangeItem(itemBase);
        if (holdingHand != null)
            heldItemWorldObject.transform.SetParent(holdingHand.transform);
    }

    public void OnUse(InputAction.CallbackContext value)
    {
        if (!isLocalPlayer) return;
        //Checks if button is pressed and if there is an item in the player hand triggers that items use function
        if (value.performed)
        {
            if (itemBase != null)
            {
                currentActingComponent.Use(itemBase);
            }
        }
    }

    //Changes what item the player has in their hand and sets the correct mesh and material
    public void ChangeItem(ItemBase newItemBase)
    {
        itemBase = newItemBase;
        Type itemType = Type.GetType(itemBase.GetItemBaseBehaviorScriptName);
        if (currentActingComponent != null)
        {
            currentActingComponent.StopAnimation();
            Destroy(currentActingComponent);
        }

        currentActingComponent = (ItemBaseBehaviour) gameObject.AddComponent(itemType);
        currentActingComponent.SetBelongingTo(itemBase);
        currentActingComponentType = itemType;
        heldItemWorldObject.GetComponent<MeshFilter>().mesh = itemBase.GetMesh;
        heldItemWorldObject.GetComponent<MeshRenderer>().material = itemBase.GetMaterial;
    }

    public void SyncHeldItem(int index)
    {
        itemBase = gameObject.GetComponent<PlayerInventory>().inventory[index];
        heldItemWorldObject.GetComponent<MeshFilter>().mesh = itemBase.GetMesh;
        heldItemWorldObject.GetComponent<MeshRenderer>().material = itemBase.GetMaterial;
    }
}