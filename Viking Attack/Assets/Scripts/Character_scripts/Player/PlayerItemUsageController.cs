using ItemNamespace;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

public class PlayerItemUsageController : NetworkBehaviour
{
    public ItemBase itemBase; // Will need to be updated if another item is being used.
    [SerializeField] private GameObject heldItemWorldObject;
    [SerializeField] private GameObject holdingHand;
    
    private Type currentActingComponentType;
    private ItemBaseBehavior currentActingComponent;

    public void Start()
    {
        if(holdingHand != null)
            heldItemWorldObject.transform.SetParent(holdingHand.transform);
    }

    public void OnUse(InputAction.CallbackContext value)
    {
        if (!isLocalPlayer) return;
        if (value.performed)
        { 
            if(itemBase != null)
            {
                Type itemType = Type.GetType(itemBase.GetItemBaseBehaviorScriptName);
                if(currentActingComponentType == itemType)
                    currentActingComponent.Use();
                else
                {
                    if(currentActingComponent != null)
                        Destroy(currentActingComponent);
                    currentActingComponent = (ItemBaseBehavior)gameObject.AddComponent(itemType);
                    currentActingComponent.SetBelongingTo(itemBase);
                    currentActingComponentType = itemType;
                    heldItemWorldObject.GetComponent<MeshFilter>().mesh = itemBase.GetMesh;
                    heldItemWorldObject.GetComponent<MeshRenderer>().material = itemBase.GetMaterial;
                }
            }
        }
    }
}