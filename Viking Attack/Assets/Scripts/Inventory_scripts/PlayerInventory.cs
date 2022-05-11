using System;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory_scripts
{
    public class PlayerInventory : NetworkBehaviour
    {
        [SerializeField] private ItemBase[] inventory;
        [SerializeField] private GameObject[] sprites;
        [SerializeField] private Guid itemPickupGuid;
        [SerializeField] private GameObject selectedItem;
        [SerializeField] private GameObject meatStackNumber;
        [SerializeField] public Animator animator;
        [SerializeField] private uint netID;

        [SerializeField] private GameObject itemInfoSprite;
        [SerializeField] private GameObject[] weaponStats;


        private void Start()
        {
            inventory = new ItemBase[4];
            // Registers listener for player pickups
            netID = gameObject.GetComponent<NetworkIdentity>().netId;
            EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnItemPickup, ref itemPickupGuid);
        }


        // Inserts the itembase + its sprite to the inventory array
        void OnItemPickup(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            if (playerItemPickupEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId == netID)
            {
                switch (playerItemPickupEventInfo.itemBase.GetItemType)
                {
                    case ItemBase.ItemType.Weapon:
                        
                        switch (playerItemPickupEventInfo.itemBase.GetWeaponType)
                        {
                            // Sets the inventory slot, updates globalplayerinfo, what item the player is using 
                            // in each case
                            case ItemBase.WeaponType.Sword:
                                UpdateHeldItem(0, playerItemPickupEventInfo.itemBase);
                                UpdateItemInfo(0); // updates the weapon info box with sword information
                                animator.SetTrigger("itemPOPUP");
                                break;

                            case ItemBase.WeaponType.Spear:
                                UpdateHeldItem(1, playerItemPickupEventInfo.itemBase);
                                UpdateItemInfo(1); // updates the weapon info box with spear information
                                animator.SetTrigger("itemPOPUP");
                                break;

                            case ItemBase.WeaponType.Dagger:
                                UpdateHeldItem(2, playerItemPickupEventInfo.itemBase);
                                UpdateItemInfo(2); // updates the weapon info box with dagger information
                                animator.SetTrigger("itemPOPUP");
                                break;
                        }

                        break;
                    // END OF INNER WEAPON SWITCH
                    // In case it is a food item being picked up
                    case ItemBase.ItemType.Food:
                        if (inventory[3] != null)
                        {
                            gameObject.GetComponent<GlobalPlayerInfo>().IncreaseMeatStackNumber();
                            meatStackNumber.GetComponent<Text>().text =
                                gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber().ToString();
                        }
                        else
                        {
                            gameObject.GetComponent<GlobalPlayerInfo>().IncreaseMeatStackNumber();
                            inventory[3] = playerItemPickupEventInfo.itemBase;
                            sprites[3].SetActive(true);
                            sprites[3].GetComponent<Image>().sprite = inventory[3].GetSprite;
                            meatStackNumber.GetComponent<Text>().text =
                                gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber().ToString();
                            gameObject.GetComponent<GlobalPlayerInfo>()
                                .SetItemSlot(3, inventory[3]); // sets the info in globalplayerinfo
                        }

                        break;
                    default:
                        // code block
                        break;
                }
            }
            else
            {
                gameObject.GetComponent<PlayerItemUsageController>()
                    .ChangeItem(playerItemPickupEventInfo.itemBase);
            }
        }

        private void UpdateHeldItem(int index, ItemBase itemBase)
        {
            inventory[index] = itemBase;
            sprites[index].SetActive(true);
            sprites[index].GetComponent<Image>().sprite = inventory[index].GetSprite;
            gameObject.GetComponent<GlobalPlayerInfo>()
                .SetItemSlot(index, inventory[index]); // sets the info in globalplayerinfo
            selectedItem.transform.position =
                sprites[index].transform.position + new Vector3(0f,10f,0f);
            gameObject.GetComponent<PlayerItemUsageController>()
                .ChangeItem(itemBase);
        }

        // Updates the info box to contain the information of the weapon
        private void UpdateItemInfo(int index)
        {

            itemInfoSprite.GetComponent<Image>().sprite = sprites[index].GetComponent<Image>().sprite;
            weaponStats[0].GetComponent<Text>().text = inventory[index].GetDamage.ToString();
            weaponStats[1].GetComponent<Text>().text = inventory[index].GetRange.ToString();
            weaponStats[2].GetComponent<Text>().text = inventory[index].GetSpeed.ToString();
        }

        public void ToggleSword(InputAction.CallbackContext value)
        {
            if (!isLocalPlayer)
            {
                
                return;
            }

            if (inventory[0] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[0]);
                selectedItem.transform.position = sprites[0].transform.position + new Vector3(0f,10f,0f);
                UpdateItemInfo(0); // updates the weapon info box with sword information
                animator.SetTrigger("itemPOPUP");
            }
        }

        public void ToggleSpear(InputAction.CallbackContext value)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (inventory[1] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[1]);
                selectedItem.transform.position = sprites[1].transform.position + new Vector3(0f,10f,0f);
                UpdateItemInfo(1); // updates the weapon info box with spear information
                animator.SetTrigger("itemPOPUP");
            }
        }

        public void ToggleDagger(InputAction.CallbackContext value)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (inventory[2] != null)
            {
                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[2]);
                selectedItem.transform.position = sprites[2].transform.position + new Vector3(0f,10f,0f);
                UpdateItemInfo(2); // updates the weapon info box with dagger information
                animator.SetTrigger("itemPOPUP");
                
            }
        }

        public void ToggleFood(InputAction.CallbackContext value)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (inventory[3] != null && gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber() > 0)
            {
                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[3]);
                selectedItem.transform.position = sprites[3].transform.position + new Vector3(0f,10f,0f);
                
                //animator.SetTrigger("itemPOPUP");
            }
        }

        public void ReturnToDefault()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[0]);
            selectedItem.transform.position = sprites[0].transform.position + new Vector3(0f,10f,0f);
            UpdateItemInfo(0);
            animator.SetTrigger("itemPOPUP");
        }

        public void UpdateMeatStack()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            meatStackNumber.GetComponent<Text>().text =
                gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber().ToString();
        }
    }
}