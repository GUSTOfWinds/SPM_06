using ItemNamespace;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

public class PlayerItemUsageController : NetworkBehaviour
{
    [SerializeField] private ItemBase itemBase; // Will need to be updated if another item is being used.
    private List<Type> hasComponent = new List<Type>();
    private ItemBaseBehavior currentActingComponent;

    public void OnUse(InputAction.CallbackContext value)
    {
        if (!isLocalPlayer) return;
        if (value.performed)
        { 
            Type itemType = Type.GetType(itemBase.GetItemBaseBehaviorScriptName);
            if(hasComponent.Contains(itemType))
                currentActingComponent.Use();
            else
            {
                currentActingComponent = (ItemBaseBehavior)gameObject.AddComponent(itemType);
                currentActingComponent.SetBelongingTo(itemBase);
                hasComponent.Add(itemType);
            }
        }
    }
}