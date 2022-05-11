using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Event;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;
using UnityEngine.UI;

public class PlayerDamageUIListener : MonoBehaviour
{
    [SerializeField]
    private uint netID; // the netID of the player, making sure to only play sounds when the local player is hit

    [SerializeField] private Image img;
    [SerializeField] private Animator animator;

    private Guid uiEventGuid;

    private void Start()
    {
        EventSystem.Current.RegisterListener<DamageEventInfo>(OnPlayerDamage,
            ref uiEventGuid); // registers the listener
    }

    // Will play a random track from the array above when the local player takes damage
    void OnPlayerDamage(DamageEventInfo eventInfo)
    {
        animator = eventInfo.target.transform.Find("UI").transform.Find("Health_bar").GetComponent<Animator>();
        img = eventInfo.target.transform.Find("UI").Find("Panel").GetComponent<Image>();
        animator.SetTrigger("takeDMG");
        StartCoroutine(FadeImage());
    }

    // Sets teh alpha of the image to 0.5 and then back to 0 after the player is hit
    private IEnumerator FadeImage()
    {
        // fade from transparent to opaque
        for (float i = 0; i <= 0; i += Time.fixedDeltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 0, 0, i);
            yield return null;
        }

        // fade from opaque to transparent
        for (float i = 5f; i >= 0; i -= Time.fixedDeltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 0, 0, i);
            yield return null;
        }

        img.color = new Color(1,0,0,0);
    }


    // [ClientRpc]
    // void RpcPlayTakingDamage(DamageEventInfo damageEventInfo)
    // {

    // }
}