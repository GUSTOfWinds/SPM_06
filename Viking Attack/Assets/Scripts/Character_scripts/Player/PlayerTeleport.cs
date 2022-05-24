﻿using System;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    
    /**
     * @author Martin Kings
     */
    public class PlayerTeleport : NetworkBehaviour
    {
        [SerializeField] private GameObject teleportSpot;
        private Vector3 portPosition;

        private void Start()
        {
            portPosition = teleportSpot.transform.position;
        }

        public void StartTeleport()
        {
            if (isServer)
            {
                RpcStartConfirmation();
            }
            else
            {
                CmdStartConfirmation();
            }
            
        }

        [Command]
        private void CmdStartConfirmation()
        {
            
        }
        
        [ClientRpc]
        private void RpcStartConfirmation()
        {
            
        }
        
    }
}