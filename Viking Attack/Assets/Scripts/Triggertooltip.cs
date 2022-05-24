using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;

public class Triggertooltip : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private GameObject[] players;

    [SerializeField] private string[] firstText;

    [SerializeField] private string[] secondText;

    private string[] textToDisplay;

    [SerializeField] private bool keyIsFound;

    private Guid connectedEventGuid;

    private Guid itemEventGuid;

    private GameObject goalText;

    private GoalTextScript goalTextScript;
    
    


    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        EventSystem.Current.RegisterListener<ItemDropEventInfo>(OnItemDrop,
            ref itemEventGuid); // registers the listener
        EventSystem.Current.RegisterListener<PlayerConnectEventInfo>(UpdatePlayerList, ref connectedEventGuid);
    }

    // Players 
    void UpdatePlayerList(PlayerConnectEventInfo playerConnectEventInfo)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (keyIsFound == false)
        {
            textToDisplay = firstText;
        }
        else
        {
            textToDisplay = secondText;
        }

        goalText = other.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
        if (goalText.active == false)
        {
            goalText.SetActive(true);
            goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(textToDisplay[0], textToDisplay[1]);
        }
    }

    
    // When a player enters the proximity of the portal, the correct information will be
    // shown based on the key being picked up or not.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }


        if (keyIsFound == false)
        {
            goalText = other.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
            goalTextScript = goalText.GetComponent<GoalTextScript>();
            if (goalText.active && goalTextScript.GetTextOne() == firstText[0] &&
                goalTextScript.GetTextTwo() == firstText[1])
            {
                goalText.SetActive(false);
            }
        }
    }

    public void OnItemDrop(ItemDropEventInfo unitDeathEventInfo)
    {
        if (unitDeathEventInfo.itemBase.GetItemType == ItemBase.ItemType.Key)
        {
            keyIsFound = true;
            foreach (var player in players)
            {
                goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
                goalTextScript = goalText.GetComponent<GoalTextScript>();
                if (goalText.active)
                {
                    if (goalTextScript.GetTextOne() == firstText[0] &&
                        goalTextScript.GetTextTwo() == firstText[1])
                    {
                        goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(secondText[0], secondText[1]);
                    }
                    else
                    {
                        goalText.SetActive(false);
                    }
                }
            }
            
            // TODO activate the animation of the portal here
            
        }
    }

    public bool GetKeyStatus()
    {
        return keyIsFound;
    }
}