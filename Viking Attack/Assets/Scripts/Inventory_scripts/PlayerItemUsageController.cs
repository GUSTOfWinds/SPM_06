using ItemNamespace;
using UnityEngine;
using Mirror;
using System;
using Inventory_scripts;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerItemUsageController : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    public ItemBase itemBase; // Will need to be updated if another item is being used.

    [SerializeField] private GameObject heldItemWorldObject;

    private Type currentActingComponentType;
    private ItemBaseBehaviour currentActingComponent;
    private bool whenUsingItem;


    public void Start()
    {
        ChangeItem(itemBase);
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
                StartCoroutine(slowOnHit());
            }
        }
    }

    //Changes what item the player has in their hand and sets the correct mesh and material
    public void ChangeItem(ItemBase newItemBase)
    {
        itemBase = newItemBase; // updates itembase
        Type itemType = Type.GetType(itemBase.GetItemBaseBehaviorScriptName); // fetches type of the itembehaviour


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

    // Will be run by both client and server, only updates the mesh shown in the other players hand
    public void SyncHeldItem(int index, uint netid)
    {
        if (gameObject.GetComponent<NetworkIdentity>().netId == netid)
        {
            itemBase = gameObject.GetComponent<PlayerInventory>().inventory[index];
            heldItemWorldObject.GetComponent<MeshFilter>().mesh = itemBase.GetMesh;
            heldItemWorldObject.GetComponent<MeshRenderer>().material = itemBase.GetMaterial;
        }
    }

    public IEnumerator slowOnHit()
    {
        gameObject.transform.GetComponent<PlayerScript3D>().acceleration *= itemBase.GetSpeedMultiplierWhenUsingItem;
        yield return new WaitForSeconds(1);
        gameObject.transform.GetComponent<PlayerScript3D>().acceleration /= itemBase.GetSpeedMultiplierWhenUsingItem;
    }
}