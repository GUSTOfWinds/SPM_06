using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using Mirror;
using UnityEngine;
using UnityEngine.UI;


public class PlayLevelAnimation : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Animator parentAnimator;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator otherAnimator;
    [SerializeField] private Animator thirdAnimator;
    private Guid levelUpGuid;
    private uint netID;

    void Start()
    {
        EventSystem.Current.RegisterListener<PlayerLevelUpEventInfo>(OnPlayerLevelUp, ref levelUpGuid);
        netID = player.GetComponent<NetworkIdentity>().netId;
    }

    public void OnPlayerLevelUp(PlayerLevelUpEventInfo playerLevelUpEventInfo)
    {
        if (playerLevelUpEventInfo.netID == netID)
        {
            parentAnimator.SetTrigger("incLVL");
            otherAnimator.SetBool("levelNOTIF", true);
            thirdAnimator.SetBool("pointsavailable", true);
        }
    }
}