using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;

public class Triggertooltip : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private GameObject[] players;

    [SerializeField] private string[] firstText;

    [SerializeField] private string[] secondText;

    private string[] textToDisplay;

    [SyncVar] [SerializeField] private bool keyIsFound;

    private Guid connectedEventGuid;

    private Guid itemEventGuid;

    private GameObject goalText;

    private GoalTextScript goalTextScript;


    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnKeyPickup,
            ref itemEventGuid); // registers the listener
        EventSystem.Current.RegisterListener<PlayerConnectEventInfo>(UpdatePlayerList, ref connectedEventGuid);
    }

    // Players 
    void UpdatePlayerList(PlayerConnectEventInfo playerConnectEventInfo)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // When a player enters the collider of the object and neither of the players has picked up the 
    // relic, a text will be shown to the player to keep looking around the island
    // if it has been found, the text will state that the person can enter the portal
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


    // When a player leaves the proximity of the portal, information will be removed from the screen
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

    // When either a client or a host has picked up the relic, bools will be set and text will be displayed as 
    // requested
    public void OnKeyPickup(PlayerItemPickupEventInfo pickupEventInfo)
    {
        if (pickupEventInfo.itemBase.GetItemType == ItemBase.ItemType.Key)
        {
            // if host, update text for both player objects and then push it to the client as well
            if (isServer)
            {
                keyIsFound = true;
                foreach (var player in players)
                {
                    goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
                    goalTextScript = goalText.GetComponent<GoalTextScript>();
                    goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(secondText[0], secondText[1]);
                    StartCoroutine(DisplayText(player));
                    RpcDisplayText(player);
                }
            }
            // if client, update text for both player objects and then push it to the host as well
            else
            {
                keyIsFound = true;
                foreach (var player in players)
                {
                    goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
                    goalTextScript = goalText.GetComponent<GoalTextScript>();
                    goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(secondText[0], secondText[1]);
                    StartCoroutine(DisplayText(player));
                    CmdDisplayText(player);
                }
            }
            
            // TODO activate the animation of the portal animation or whatever here
        }
    }

    // Run by the client, forces the server to run the code within for each player object
    [Command(requiresAuthority = false)]
    private void CmdDisplayText(GameObject player)
    {
        goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
        goalTextScript = goalText.GetComponent<GoalTextScript>();
        goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(secondText[0], secondText[1]);
        keyIsFound = true;
        StartCoroutine(DisplayText(player));
    }

    // Run by the host, forces the clients to run the code within for each player object
    [ClientRpc]
    private void RpcDisplayText(GameObject player)
    {
        if (!isServer)
        {
            goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
            goalTextScript = goalText.GetComponent<GoalTextScript>();
            goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(secondText[0], secondText[1]);
            keyIsFound = true;
            StartCoroutine(DisplayText(player));
        }
    }

    // Displays the tooltip text for the player it runs on 
    // and lets the player know to approach the portal
    private IEnumerator DisplayText(GameObject player)
    {
        player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject.SetActive(true);
        yield return new WaitForSeconds(7);
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) > 15f)
        {
            player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject.SetActive(false);
        }
    }

    public bool GetKeyStatus()
    {
        return keyIsFound;
    }
    //for saving the key status, if the key is found , when we load, i want to change the key status here
    public void setKeyStatus() { keyIsFound = true; } //By Jiang
}