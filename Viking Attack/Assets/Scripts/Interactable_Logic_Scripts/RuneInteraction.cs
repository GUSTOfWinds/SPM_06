using System.Collections.Generic;
using Mirror;
using UnityEngine;


    public class RuneInteraction : BaseObjectInteraction
    {
        private List<GlobalPlayerInfo> interacted = new List<GlobalPlayerInfo>();
        //The object that the lever is activating
        [SerializeField] private GameObject activationObject;
        //A boolean if a player can get a point or not
        private bool canGetPoint;
        //Sets to the starting rotation
        private Quaternion targetRotation;

        [SerializeField] private GameObject colliderToDisable;
        [SerializeField] private GameObject colliderToEnable;


        public override void InteractedWith(GameObject playerThatInteracted)
        {
            if (playerThatInteracted != isLocalPlayer) return;
            canGetPoint = !interacted.Contains(playerThatInteracted.GetComponent<GlobalPlayerInfo>());
            //Calls the object to activate (uses the BaseObjectActivation so i can call different objects)
            activationObject.GetComponent<BaseObjectActivation>().activate();

            GlobalPlayerInfo playerInfo = playerThatInteracted.GetComponent<GlobalPlayerInfo>();
            
            //Gives player a level if they  don't exist within the list, otherwise no.
            if(canGetPoint)
            {
                playerInfo.IncreaseLevel();
                playerInfo.SetHealth(playerInfo.GetMaxHealth());
                colliderToDisable.SetActive(false);
                colliderToEnable.SetActive(true);

            }else
            {
                playerInfo.SetHealth(playerInfo.GetMaxHealth());
            }

            if (!interacted.Contains(playerInfo))
            {
                interacted.Add(playerInfo);
            }
        }
        // Sets the default targetRotation to current LeverShaftPivot rotation
    }

