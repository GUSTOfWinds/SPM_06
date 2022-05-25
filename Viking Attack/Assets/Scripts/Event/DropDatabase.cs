using System;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;


namespace Event
{
    public class DropDatabase : MonoBehaviour
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private List<ItemBase> droppedItems; // Contains all sounds that can be played

        private Guid itemEventGuid;

        private void Start()
        {
            EventSystem.Current.RegisterListener<ItemDropEventInfo>(OnItemDrop,
                ref itemEventGuid); // registers the listener
        }

        
        void OnItemDrop(ItemDropEventInfo eventInfo)
        {
            if (eventInfo.itemBase.GetName != "Meat" && GetIsDropped(eventInfo.itemBase) == false)
            {
                droppedItems.Add(eventInfo.itemBase);
            }
        }

        public bool GetIsDropped(ItemBase itemBase)
        {
            return droppedItems.Contains(itemBase);
        }
    }
}