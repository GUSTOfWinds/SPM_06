using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayLevelAnimation : MonoBehaviour
{
    [SerializeField] private Animator parentAnimator;
    [SerializeField] private Animator otherAnimator;
    private Guid levelUpGuid;
    void Start()
    {
        EventSystem.Current.RegisterListener<PlayerLevelUpEventInfo>(OnPlayerLevelUp, ref levelUpGuid);
    }

    public void OnPlayerLevelUp(PlayerLevelUpEventInfo playerLevelUpEventInfo)
    {
        parentAnimator.SetTrigger("incLVL");
        otherAnimator.SetBool("levelNOTIF", true);
    }
    
}
