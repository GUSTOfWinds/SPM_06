using System.Collections;
using Event;
using Inventory_scripts;
using ItemNamespace;
using UnityEngine;
using Mirror;

public class ItemInteraction : BaseObjectInteraction
{
    public override void InteractedWith(GameObject playerThatInteracted)
    {
        playerThatInteracted.GetComponent<PlayerItemUsageController>().itemBase =
            gameObject.GetComponent<DropItemInWorldScript>().itembase;


        // Activates an event that the inventory will pick up and add the item to the inventory
        EventInfo itemPickUpEvent = new PlayerItemPickupEventInfo
        {
            EventUnitGo = playerThatInteracted,
            itemToDestroy = gameObject,
            itemBase = gameObject.GetComponent<DropItemInWorldScript>().itembase
        };
        EventSystem.Current.FireEvent(itemPickUpEvent);

        if (isClientOnly)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}