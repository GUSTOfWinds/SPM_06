using System;
using System.Net.Mime;
using Event;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace ItemNamespace
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemBase[] inventory;
        [SerializeField] private GameObject[] sprites;
        [SerializeField] private Guid itemPickupGuid;
        

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
                    
                    switch (weaponType)
                    {
                        case ItemBase.WeaponType.Sword:
                            inventory[0] = playerItemPickupEventInfo.itemBase;
                            sprites[0].SetActive(true);
                            sprites[0].GetComponent<Image>().sprite = inventory[0].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(0, inventory[0]); // sets the info in globalplayerinfo
                            break;

                        case ItemBase.WeaponType.Spear:
                            inventory[1] = playerItemPickupEventInfo.itemBase;
                            sprites[1].SetActive(true);
                            sprites[1].GetComponent<Image>().sprite = inventory[1].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(1, inventory[1]); // sets the info in globalplayerinfo
                            break;
                        
                        case ItemBase.WeaponType.Dagger:
                            inventory[2] = playerItemPickupEventInfo.itemBase;
                            sprites[2].SetActive(true);
                            sprites[2].GetComponent<Image>().sprite = inventory[2].GetSprite;
                            gameObject.GetComponent<GlobalPlayerInfo>().SetItemSlot(2, inventory[2]); // sets the info in globalplayerinfo
                            break;
                    }
                    // code block
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

        // void ToggleItem(int slot)
        // {
        //     gameObject
        // }
        
    }
}