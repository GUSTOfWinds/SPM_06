using System.Collections;
using Event;
using Inventory_scripts;
using ItemNamespace;
using Mirror;
using UnityEngine;


    public class ItemInteraction : BaseObjectInteraction
    {
        /**
         * @author Martin Kings
         */
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

            if (gameObject.GetComponent<DropItemInWorldScript>().itembase.GetItemType == ItemBase.ItemType.Key)
            {
                StartCoroutine(DestroyAfterWait());
                return;
            }
            
            if (isClientOnly)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private IEnumerator DestroyAfterWait()
        {
            yield return new WaitForSeconds(0.2f);
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                CmdDeleteKey();
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdDeleteKey()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
