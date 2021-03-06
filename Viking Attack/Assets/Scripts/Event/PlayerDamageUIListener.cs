using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Event
{
    public class PlayerDamageUIListener : NetworkBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField]
        private uint netID; // the netID of the player, making sure to only play sounds when the local player is hit

        [SerializeField] private Image img;
        [SerializeField] private Animator animator;

        private Guid uiEventGuid;

        private void Start()
        {
            netID = gameObject.GetComponent<NetworkIdentity>().netId;
            EventSystem.Current.RegisterListener<PlayerDamageEventInfo>(OnPlayerDamage,
                ref uiEventGuid); // registers the listener
        }

        // Will play an animation on the client being hit.
        void OnPlayerDamage(PlayerDamageEventInfo eventInfo)
        {
            if (isServer)
            {
                if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID)
                {
                    if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID)
                    {
                        animator = gameObject.transform.Find("UI").transform.Find("Health_bar").GetComponent<Animator>();
                        img = gameObject.transform.Find("UI").Find("Panel").GetComponent<Image>();
                        animator.SetTrigger("takeDMG");
                        StartCoroutine(FadeImage());
                    
                        RpcOnPlayerDamage(eventInfo);
                    }
                }
            }
        }

        [ClientRpc]
        void RpcOnPlayerDamage(PlayerDamageEventInfo eventInfo)
        {
            if (isServer)
            {
                return;
            }
            if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID)
            {
                animator = eventInfo.target.transform.Find("UI").transform.Find("Health_bar").GetComponent<Animator>();
                img = eventInfo.target.transform.Find("UI").Find("Panel").GetComponent<Image>();
                animator.SetTrigger("takeDMG");
                StartCoroutine(FadeImage());
            }
        }

        // Sets teh alpha of the image to 0.5 and then back to 0 after the player is hit
        private IEnumerator FadeImage()
        {
            // fade from transparent to opaque
        
            img.color = new Color(1, 0, 0, 1);
            yield return null;
        

            // fade from opaque to transparent
            for (float i = 5f; i >= 0; i -= Time.fixedDeltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 0, 0, i);
                yield return null;
            }

            img.color = new Color(1, 0, 0, 0);
        }
    }
}