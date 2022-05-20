using System;
using Event;
using ItemNamespace;
using UnityEngine;

public class Triggertooltip : MonoBehaviour
{
    [SerializeField] private GameObject[] players;

    [SerializeField] private string[] firstText;

    [SerializeField] private string[] secondText;

    private string[] textToDisplay;

    [SerializeField] private bool bossIsDead;

    private Guid connectedEventGuid;

    private Guid portalEventGuid;


    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        EventSystem.Current.RegisterListener<UnitDeathEventInfo>(SetBossLifeStatus, ref portalEventGuid);
        EventSystem.Current.RegisterListener<PlayerConnectEventInfo>(UpdatePlayerList, ref connectedEventGuid);
    }

    // Players 
    void UpdatePlayerList(PlayerConnectEventInfo playerConnectEventInfo)
    {
        Debug.Log("Jag uppdateras när spelare 2 kommer");
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (bossIsDead == false)
        {
            textToDisplay = firstText;
        }
        else
        {
            textToDisplay = secondText;
        }

        if (other.CompareTag("Player"))
        {
            GameObject goalText = other.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
            if (goalText.active == false)
            {
                goalText.SetActive(true);
                goalText.GetComponent<GoalTextScript>().UpdateTextAndDisplay(textToDisplay[0], textToDisplay[1]);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bossIsDead == false)
        {
            if (other.CompareTag("Player"))
            {
                GameObject goalText = other.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
                GoalTextScript goalTextScript = goalText.GetComponent<GoalTextScript>();
                if (goalText.active && goalTextScript.GetTextOne() == firstText[0] &&
                    goalTextScript.GetTextTwo() == firstText[1])
                {
                    goalText.SetActive(false);
                }
            }
        }
    }

    public void SetBossLifeStatus(UnitDeathEventInfo unitDeathEventInfo)
    {
        if (unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetName() == "Boss")
        {
            bossIsDead = true;
            foreach (var player in players)
            {
                GameObject goalText = player.transform.Find("UI").gameObject.transform.Find("GoalText").gameObject;
                GoalTextScript goalTextScript = goalText.GetComponent<GoalTextScript>();
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
        }
    }
}