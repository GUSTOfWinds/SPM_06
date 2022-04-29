using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LeverInteraction : BaseObjectInteraction
{
    [SerializeField] private float roatitionSpeed;
    //The object that the lever is activating
    [SerializeField] private GameObject activationObject;
    //A boolean if the lever is on or off
    private bool leverOn;
    //Sets to the starting rotation
    private Quaternion targetRotation;
    //Is called from InteractableObjectScript when the player press the chosen button
    private Transform leverPivot;

    public override void InteractedWith()
    {
        //Calls the object to activate (uses the BaseObjectActivation so i can call different objects)
        activationObject.GetComponent<BaseObjectActivation>().activate();
        //Moves the lever shaft by 90 degrees
        if(!leverOn)
        {
            targetRotation = Quaternion.Euler(transform.parent.transform.rotation.x,transform.parent.transform.rotation.y,-45);
            leverOn = true;
        }else
        {
            targetRotation = Quaternion.Euler(transform.parent.transform.rotation.x,transform.parent.transform.rotation.y, 45);
            leverOn = false;
        }   
    }
    // Sets the default targetRotation to current LeverShaftPivot rotation
    private void Start()
    {
        leverPivot = transform.GetChild(1);
        targetRotation = syncObject.FindObjectWithTag("LeverShaftPivot").transform.rotation;
        //targetRotation = transform.FindObjectWithTag("LeverShaftPivot").transform.rotation;
        
    }

    private SyncObject syncObject = new SyncObject();

    void Update()
    {
        if (!syncObject.IsItLocal())
        {
            base.transform.rotation = syncObject.syncRotation;
            return;
        }
        //Moves the lever in a motion (Not teleporting)
        leverPivot.rotation = Quaternion.RotateTowards(leverPivot.rotation, targetRotation, roatitionSpeed * Time.deltaTime);
        syncObject.CmdSetSynchedRotation(transform.rotation);
    }

    private class SyncObject : NetworkBehaviour
    {
        [SyncVar] public Quaternion syncRotation;
        
        
        [Command]
        public void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;


        public bool IsItLocal()
        {
            return isLocalPlayer;
        }

        private GameObject returnObject;

        public GameObject FindObjectWithTag(String findString)
        {
            return FindObjectWithTag(findString);
        }
    }
}
