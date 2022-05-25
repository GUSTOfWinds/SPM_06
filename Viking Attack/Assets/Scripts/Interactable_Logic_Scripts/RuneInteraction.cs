using System.Collections.Generic;
using Mirror;
using UnityEngine;


    public class RuneInteraction : BaseObjectInteraction
    {
        private List<GlobalPlayerInfo> interacted = new List<GlobalPlayerInfo>();
        [SerializeField] private float roatitionSpeed;
        //The object that the lever is activating
        [SerializeField] private GameObject activationObject;
        //A boolean if a player can get a point or not
        private bool canGetPoint;
        //Sets to the starting rotation
        private Quaternion targetRotation;

        public override void InteractedWith(GameObject playerThatInteracted)
        {
            canGetPoint = interacted.Contains(playerThatInteracted.GetComponent<GlobalPlayerInfo>());
            //Calls the object to activate (uses the BaseObjectActivation so i can call different objects)
            activationObject.GetComponent<BaseObjectActivation>().activate();
            
            
            //Gives player a level if they exist within the list, otherwise no.
            if(canGetPoint)
            {
                playerThatInteracted.GetComponent<GlobalPlayerInfo>().IncreaseLevel();

            }else
            {
            }   
        }
        // Sets the default targetRotation to current LeverShaftPivot rotation
    }

