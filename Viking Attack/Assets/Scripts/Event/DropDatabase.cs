using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Event;
using ItemNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropDatabase : MonoBehaviour
{
    [SerializeField] private List<ItemBase> droppedItems; // Contains all sounds that can be played

    private Guid itemEventGuid;

    private void Start()
    {
        EventSystem.Current.RegisterListener<ItemDropEventInfo>(OnItemDrop,
            ref itemEventGuid); // registers the listener
    }

    // Will play a random track from the array above when the local player takes damage
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