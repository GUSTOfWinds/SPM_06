﻿using System;
using System.Net.Mime;
using Event;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ItemNamespace
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemBase[] inventory;
        [SerializeField] private GameObject[] sprites;
        [SerializeField] private Guid itemPickupGuid;
        [SerializeField] private GameObject selectedItem;
        

        private void Start()
        {
            inventory = new ItemBase[4];

            EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnItemPickup, ref itemPickupGuid);
        }

        
        // Inserts the itembase + its sprite to the inventory array
        void OnItemPickup(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            ItemBase.ItemType itemBase = playerItemPickupEventInfo.itemBase.GetItemType;
            switch(itemBase) 
            {
                case ItemBase.ItemType.Weapon:
                    
                    ItemBase.WeaponType weaponType = playerItemPickupEventInfo.itemBase.GetWeaponType;
                    
                    
                    // TODO ADD THAT HELD ITEM IS UPDATED TO THE WEAPON THAT IS PICKED UP
                    switch (weaponType)
                    {
                        case ItemBase.WeaponType.Sword:
                            inventory[0] = playerItemPickupEventInfo.itemBase;
                            sprites[0].SetActive(true);
                            sprites[0].GetComponent<Image>().sprite = inventory[0].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(0, inventory[0]); // sets the info in globalplayerinfo
                            selectedItem.transform.position = sprites[0].transform.position;
                            gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[0];
                            break;

                        case ItemBase.WeaponType.Spear:
                            inventory[1] = playerItemPickupEventInfo.itemBase;
                            sprites[1].SetActive(true);
                            sprites[1].GetComponent<Image>().sprite = inventory[1].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(1, inventory[1]); // sets the info in globalplayerinfo
                            selectedItem.transform.position = sprites[1].transform.position;
                            gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[1];
                            break;
                        
                        case ItemBase.WeaponType.Dagger:
                            inventory[2] = playerItemPickupEventInfo.itemBase;
                            sprites[2].SetActive(true);
                            sprites[2].GetComponent<Image>().sprite = inventory[2].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(2, inventory[2]); // sets the info in globalplayerinfo
                            selectedItem.transform.position = sprites[2].transform.position;
                            gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[2];
                            break;
                    }
                    break;
                // END OF INNER WEAPON SWITCH
                
                case ItemBase.ItemType.Food:
                    inventory[3] = playerItemPickupEventInfo.itemBase;
                    sprites[3].SetActive(true);
                    sprites[3].GetComponent<Image>().sprite = inventory[3].GetSprite;
                    gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(3, inventory[3]); // sets the info in globalplayerinfo
                    break;
                default:
                    // code block
                    break;
            }
        }

        public void ToggleSword(InputAction.CallbackContext value)
        {
            if (inventory[0] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[0];
                selectedItem.transform.position = sprites[0].transform.position;
            }
            
        }
        public void ToggleSpear(InputAction.CallbackContext value)
        {
            if (inventory[1] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[1];
                selectedItem.transform.position = sprites[1].transform.position;
            }
        }
        public void ToggleDagger(InputAction.CallbackContext value)
        {
            if (inventory[2] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[2];
                selectedItem.transform.position = sprites[2].transform.position;
            }
        }
        public void ToggleFood(InputAction.CallbackContext value)
        {
            if (inventory[3] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().itemBase = inventory[3];
                selectedItem.transform.position = sprites[3].transform.position;
            }
        }
        
        
    }
}