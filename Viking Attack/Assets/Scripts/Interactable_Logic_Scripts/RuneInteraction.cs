using System.Collections.Generic;
using Mirror;
using UnityEngine;
/**
 * @author Victor Wikner
 */

    public class RuneInteraction : BaseObjectInteraction
    {
        //A boolean if a player can get a point or not
        private bool canGetPoint = true;

        //Disables and enables collider to show proper text or not
        [SerializeField] private GameObject colliderToDisable;
        [SerializeField] private GameObject colliderToEnable;


        public override void InteractedWith(GameObject playerThatInteracted)
        {
            GlobalPlayerInfo playerInfo = playerThatInteracted.GetComponent<GlobalPlayerInfo>();

            if (!playerInfo.isLocalPlayer) return;

            if(canGetPoint == true)
            {
                playerInfo.IncreaseLevel();
                colliderToDisable.SetActive(false);
                colliderToEnable.SetActive(true);
                canGetPoint = false;

            }
            playerInfo.UpdateHealth(playerInfo.GetMaxHealth());

        }
    }

