using System;
using System.Globalization;
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
        /**
         * @author Martin Kings
         */
        [SerializeField] public ItemBase[] inventory;

        [SerializeField] private GameObject[] sprites;
        [SerializeField] private GameObject selectedItem;
        [SerializeField] private GameObject meatStackNumber;
        [SerializeField] public Animator animator;
        [SerializeField] private uint netID;
        [SerializeField] private GameObject itemInfoSprite;
        [SerializeField] private GameObject[] weaponStats;
        [SerializeField] private PlayerItemUsageController playerItemUsageController;
        [SerializeField] private ItemBase wieldedItemBase;

        private Guid itemPickupGuid;
        private static readonly int ItemPopup = Animator.StringToHash("itemPOPUP");


        private void Start()
        {
            // Registers listener for player pickups
            playerItemUsageController = gameObject.GetComponent<PlayerItemUsageController>();
            netID = gameObject.GetComponent<NetworkIdentity>().netId;
            EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnItemPickup, ref itemPickupGuid);
        }


        // Inserts the itemBase + its sprite to the inventory array
        void OnItemPickup(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            if (playerItemPickupEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId == netID)
            {
                switch (playerItemPickupEventInfo.itemBase.GetItemType)
                {
                    case ItemBase.ItemType.Weapon:
                        int weaponIndex = 0;
                        switch (playerItemPickupEventInfo.itemBase.GetWeaponType)
                        {
                            case ItemBase.WeaponType.Spear:
                                weaponIndex = 1;
                                break;

                            case ItemBase.WeaponType.Dagger:
                                weaponIndex = 2;
                                break;
                        }

                        // Updates the held item locally
                        UpdateHeldItem(weaponIndex, playerItemPickupEventInfo.itemBase);
                        // Updates the weapon item information locally
                        UpdateItemInfo(weaponIndex);

                        // Syncs the held item to either server or client
                        if (isClientOnly)
                        {
                            CmdUpdateWeapon(weaponIndex, gameObject);
                        }

                        if (isServer)
                        {
                            RpcUpdateWeapon(weaponIndex, gameObject);
                        }

                        animator.SetTrigger(ItemPopup);

                        break;
                    // END OF INNER WEAPON SWITCH

                    // In case it is a food item being picked up
                    case ItemBase.ItemType.Food:
                        if (sprites[3].active)
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
                                .SetItemSlot(3, inventory[3]); // sets the info in globalPlayerInfo
                        }

                        break;

                    case ItemBase.ItemType.Armor:

                        if (sprites[4].active)
                        {
                            gameObject.GetComponent<GlobalPlayerInfo>().IncreaseArmorLevel();
                        }
                        else
                        {
                            inventory[4] = playerItemPickupEventInfo.itemBase;
                            sprites[4].SetActive(true);
                            sprites[4].GetComponent<Image>().sprite = inventory[4].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().IncreaseArmorLevel();
                            gameObject.GetComponent<GlobalPlayerInfo>()
                                .SetItemSlot(4, inventory[4]);
                        }

                        break;
                    default:
                        // code block
                        break;
                }
            }
        }


        // A call from a client to the host that the client has picked up a new weapon
        [Command(requiresAuthority = false)]
        void CmdUpdateWeapon(int index, GameObject go)
        {
            go.GetComponent<PlayerItemUsageController>().SyncHeldItem(index, go.GetComponent<NetworkIdentity>().netId);
        }


        // A call from the host to the client that the host has picked up a new weapon
        [ClientRpc]
        void RpcUpdateWeapon(int index, GameObject go)
        {
            go.GetComponent<PlayerItemUsageController>().SyncHeldItem(index, go.GetComponent<NetworkIdentity>().netId);
        }


        // Activates all parts needed for the inventory slot of the itembase that was picked up
        // to be able to function fully
        private void UpdateHeldItem(int index, ItemBase itemBase)
        {
            sprites[index].SetActive(true);
            sprites[index].GetComponent<Image>().sprite = inventory[index].GetSprite;
            gameObject.GetComponent<GlobalPlayerInfo>()
                .SetItemSlot(index, inventory[index]); // sets the info in globalplayerinfo
            selectedItem.transform.position =
                sprites[index].transform.position + new Vector3(0f, 10f, 0f);
            gameObject.GetComponent<PlayerItemUsageController>()
                .ChangeItem(itemBase);
        }

        // Updates the info box that contains the information of the weapon
        private void UpdateItemInfo(int index)
        {
            if (itemInfoSprite.transform.parent.gameObject.active == false)
            {
                itemInfoSprite.transform.parent.gameObject.SetActive(true);
            }

            itemInfoSprite.GetComponent<Image>().sprite = sprites[index].GetComponent<Image>().sprite;
            weaponStats[0].GetComponent<Text>().text = inventory[index].GetDamage.ToString();
            weaponStats[1].GetComponent<Text>().text = inventory[index].GetRange.ToString(CultureInfo.CurrentCulture);
            weaponStats[2].GetComponent<Text>().text = inventory[index].GetSpeed.ToString(CultureInfo.CurrentCulture);
        }


        // When the player presses the 1 button, the sword is toggled and worn,
        // if the sword has been picked up
        public void ToggleSword(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                if (playerItemUsageController.itemBase == inventory[0])
                {
                    return;
                }

                // Checks if the previously wielded item is active and returns if so 
                if (isLocalPlayer)
                {
                    wieldedItemBase = playerItemUsageController.itemBase;
                
                    switch (wieldedItemBase.GetItemBaseBehaviorScriptName)
                    {
                        case "ItemSpearBehaviour":
                            if (gameObject.GetComponent<ItemSpearBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemDaggerBehaviour":
                            if (gameObject.GetComponent<ItemDaggerBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemMeatBehaviour":
                            if (gameObject.GetComponent<ItemMeatBehaviour>().eating)
                            {
                                return;
                            }
                
                            break;
                    }
                }

                if (sprites[0].active)
                {
                    // Syncs the held item to either server or client
                    if (isClientOnly)
                    {
                        if (isLocalPlayer)
                        {
                            CmdUpdateWeapon(0, gameObject);
                        }
                    }

                    if (isServer)
                    {
                        if (isLocalPlayer)
                        {
                            RpcUpdateWeapon(0, gameObject);
                        }
                    }

                    if (isLocalPlayer)
                    {
                        gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[0]);
                        selectedItem.transform.position = sprites[0].transform.position + new Vector3(0f, 10f, 0f);
                        UpdateItemInfo(0); // updates the weapon info box with sword information
                        animator.SetTrigger(ItemPopup);
                    }
                }
            }
        }

        // When the player presses the 2 button, the spear is toggled and worn,
        // if the spear has been picked up
        public void ToggleSpear(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                if (playerItemUsageController.itemBase == inventory[1])
                {
                    return;
                }

                // Checks if the previously wielded item is active and returns if so 
                if (isLocalPlayer)
                {
                    wieldedItemBase = playerItemUsageController.itemBase;
                
                    switch (wieldedItemBase.GetItemBaseBehaviorScriptName)
                    {
                        case "ItemSwordBehaviour":
                            if (gameObject.GetComponent<ItemSwordBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemDaggerBehaviour":
                            if (gameObject.GetComponent<ItemDaggerBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemMeatBehaviour":
                            if (gameObject.GetComponent<ItemMeatBehaviour>().eating)
                            {
                                return;
                            }
                
                            break;
                    }
                }

                if (sprites[1].active)
                {
                    // Syncs the held item to either server or client
                    if (isClientOnly)
                    {
                        CmdUpdateWeapon(1, gameObject);
                    }

                    if (isServer)
                    {
                        RpcUpdateWeapon(1, gameObject);
                    }
                    gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[1]);
                    selectedItem.transform.position = sprites[1].transform.position + new Vector3(0f, 10f, 0f);
                    UpdateItemInfo(1); // updates the weapon info box with spear information
                    animator.SetTrigger(ItemPopup);
                }
            }
        }

        // When the player presses the 3 button, the dagger is toggled and worn,
        // if the dagger has been picked up
        public void ToggleDagger(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                if (playerItemUsageController.itemBase == inventory[2])
                {
                    return;
                }

                // Checks if the previously wielded item is active and returns if so 
                if (isLocalPlayer)
                {
                    wieldedItemBase = playerItemUsageController.itemBase;
                
                    switch (wieldedItemBase.GetItemBaseBehaviorScriptName)
                    {
                        case "ItemSwordBehaviour":
                            if (gameObject.GetComponent<ItemSwordBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemSpearBehaviour":
                            if (gameObject.GetComponent<ItemSpearBehaviour>().attackLocked)
                            {
                                return;
                            }
                
                            break;
                        case "ItemMeatBehaviour":
                            if (gameObject.GetComponent<ItemMeatBehaviour>().eating)
                            {
                                return;
                            }
                
                            break;
                    }
                }

                if (sprites[2].active)
                {
                    // Syncs the held item to either server or client
                    if (isClientOnly)
                    {
                        CmdUpdateWeapon(2, gameObject);
                    }

                    if (isServer)
                    {
                        RpcUpdateWeapon(2, gameObject);
                    }

                    gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[2]);
                    selectedItem.transform.position = sprites[2].transform.position + new Vector3(0f, 10f, 0f);
                    UpdateItemInfo(2); // updates the weapon info box with dagger information
                    animator.SetTrigger(ItemPopup);
                }
            }
        }

        // When the player presses the 4 button, the food is toggled and held in hand
        // if the player has more than 0 food in his/her inventory
        public void ToggleFood(InputAction.CallbackContext value)
        {
            if (gameObject.GetComponent<PlayerItemUsageController>().itemBase == inventory[3])
            {
                return;
            }

            // Checks if the previously wielded item is active and returns if so 
            if (isLocalPlayer)
            {
                wieldedItemBase = playerItemUsageController.itemBase;
            
                switch (wieldedItemBase.GetItemBaseBehaviorScriptName)
                {
                    case "ItemSwordBehaviour":
                        if (gameObject.GetComponent<ItemSwordBehaviour>().attackLocked)
                        {
                            return;
                        }
            
                        break;
                    case "ItemSpearBehaviour":
                        if (gameObject.GetComponent<ItemSpearBehaviour>().attackLocked)
                        {
                            return;
                        }
            
                        break;
                    case "ItemDaggerBehaviour":
                        if (gameObject.GetComponent<ItemDaggerBehaviour>().attackLocked)
                        {
                            return;
                        }
            
                        break;
                }
            }

            if (gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber() > 0)
            {
                // Syncs the held item to either server or client
                if (isClientOnly)
                {
                    CmdUpdateWeapon(3, gameObject);
                }

                if (isServer)
                {
                    RpcUpdateWeapon(3, gameObject);
                }

                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[3]);
                selectedItem.transform.position = sprites[3].transform.position + new Vector3(0f, 10f, 0f);
            }
        }


        // After eating all your food, the player goes back to holding the sword
        public void ReturnToDefault()
        {
            // Syncs the held item to either server or client
            if (isClientOnly)
            {
                CmdUpdateWeapon(0, gameObject);
            }

            if (isServer)
            {
                RpcUpdateWeapon(0, gameObject);
            }

            if (isLocalPlayer)
            {
                gameObject.GetComponent<PlayerItemUsageController>().ChangeItem(inventory[0]);
                selectedItem.transform.position = sprites[0].transform.position + new Vector3(0f, 10f, 0f);
                UpdateItemInfo(0); // updates the weapon info box with sword information
                animator.SetTrigger(ItemPopup);
            }
        }

        // Fetches the current number of food the player has and updates the text shown in the UI
        public void UpdateMeatStack()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            meatStackNumber.GetComponent<Text>().text =
                gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber().ToString();

            if (gameObject.GetComponent<GlobalPlayerInfo>().GetMeatStackNumber() < 1)
            {
                sprites[3].SetActive(false);
            }
            else
            {
                sprites[3].SetActive(true);
            }
        }
    }
}