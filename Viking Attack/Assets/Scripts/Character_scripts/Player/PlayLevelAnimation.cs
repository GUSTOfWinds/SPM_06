using System;
using Event;
using Mirror;
using UnityEngine;


public class PlayLevelAnimation : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Animator parentAnimator;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator otherAnimator;
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
        }
    }
}