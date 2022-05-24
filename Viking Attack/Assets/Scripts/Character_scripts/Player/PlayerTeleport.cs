using System;
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
        [SerializeField] private GameObject uiPanel;
        [SyncVar] [SerializeField] private bool clickedYes;
        private Vector3 portPosition;

        private void Start()
        {
            teleportSpot = GameObject.FindGameObjectWithTag("BossPortal");
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
                uiPanel.SetActive(true);
                CmdStartConfirmation();
            }
            
        }

        [Command]
        private void CmdStartConfirmation()
        {
            uiPanel.SetActive(true);
        }
        
        [ClientRpc]
        private void RpcStartConfirmation()
        {
            uiPanel.SetActive(true);
        }
        
    }
}