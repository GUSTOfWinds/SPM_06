using Event;
using Interactable_Logic_Scripts.Base;
using Inventory_scripts;
using UnityEngine;

namespace Interactable_Logic_Scripts
{
    public class ItemInteraction : BaseObjectInteraction
    {
        public override void InteractedWith(GameObject playerThatInteracted)
        {

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
}