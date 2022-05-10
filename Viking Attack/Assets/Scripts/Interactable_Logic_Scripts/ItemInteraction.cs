using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;

public class ItemInteraction : BaseObjectInteraction
{
    public override void InteractedWith(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerItemUsageController>().itemBase =
            this.gameObject.GetComponent<DropItemInWorldScript>().itembase;


        // Activates an event that the inventory will pick up and add the item to the inventory
        EventInfo itemPickUpEvent = new PlayerItemPickupEventInfo
        {
            EventUnitGo = gameObject,
            itemBase = this.gameObject.GetComponent<DropItemInWorldScript>().itembase
        };
        EventSystem.Current.FireEvent(itemPickUpEvent);
        
        Destroy(this.gameObject);
    }
}