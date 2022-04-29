using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BridgeActivation : BaseObjectActivation
{
    
    [SerializeField] private float rotationSpeed;
    //Sets to the starting rotation
    private Quaternion targetRotation;
    //A boolean if the bridge is down or upp
    private bool bridgeDown;
    
    public override void activate()
    {
        var rotation = transform.rotation;

        //Moves the bridge shaft by 90 degrees
        if(!bridgeDown)
        {
            targetRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y,0);
            bridgeDown = true;
        }else
        {
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation.eulerAngles.y,-90);
            bridgeDown = false;
        }      
    }

    void Start()
    {
        targetRotation = transform.rotation;
        
    }

    void Update()
    {
        if (!syncObject.IsItLocal())
        {
            base.transform.rotation = syncObject.syncRotation;
            return;
        }
        //Moves the bridge in a motion (Not teleporting)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        syncObject.CmdSetSynchedRotation(transform.rotation);

    }    
    
    private SyncObject syncObject = new SyncObject();

    class SyncObject : NetworkBehaviour
    {
        [SyncVar] public Quaternion syncRotation;
        
        
        [Command]
        public void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;


        public bool IsItLocal()
        {
            return isLocalPlayer;
        }
    }
    
}
