using System;
using System.Net.Mime;
using Event;
using UnityEngine;
using UnityEngine.UI;

namespace ItemNamespace
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemBase[] inventory;
        [SerializeField] private GameObject[] weaponSprites;
        [SerializeField] private Guid itemPickupGuid;

        private void Start()
        {
            inventory = new ItemBase[4];

            EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnItemPickup, ref itemPickupGuid);
        }

        void OnItemPickup(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = playerItemPickupEventInfo.itemBase;
                    weaponSprites[i].SetActive(true);
                    weaponSprites[i].GetComponent<Image>().sprite = inventory[i].GetSprite;
                    break;
                }
            }
        }
        
        
        
    }
}