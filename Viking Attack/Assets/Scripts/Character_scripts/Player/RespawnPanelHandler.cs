using System;
using Event;
using Mirror;
using Player_movement_camera_scripts;
using Player_movement_camera_scripts.Camera;
using UnityEngine;

namespace Character_scripts.Player
{
    public class RespawnPanelHandler : NetworkBehaviour
    {
        private Guid playerDeathEventGuid;
        [SerializeField] private GameObject respawnPanel;

        [SerializeField]
        private uint netID; // the netID of the player, making sure to only play when the local player is hit

        void Start()
        {
            netID = gameObject.GetComponent<NetworkIdentity>().netId; // sets the netid 
            EventSystem.Current.RegisterListener<PlayerDeathEventInfo>(OnPlayerDeath, ref playerDeathEventGuid);
        }

        // Deactivates control of player and adds death panel
        void OnPlayerDeath(PlayerDeathEventInfo playerDeathEventInfo)
        {
            if (isServer)
            {
                if (playerDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId == netID)
                {
                    gameObject.GetComponent<ToggleCharacterScreen>().locked = true;
                    gameObject.GetComponent<PlayerScript3D>().enabled = false;
                    gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = false;
                    gameObject.GetComponent<ToggleMenu>().canBeOpened = false;
                    respawnPanel.SetActive(true);
                }

                RpcOnPlayerDeath(playerDeathEventInfo);
            }
        }

        [ClientRpc]
        void RpcOnPlayerDeath(PlayerDeathEventInfo playerDeathEventInfo)
        {
            if (isServer || !isLocalPlayer) return;
            {
                if (playerDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId == netID)
                {
                    gameObject.GetComponent<ToggleCharacterScreen>().locked = true;
                    gameObject.GetComponent<PlayerScript3D>().enabled = false;
                    gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = false;
                    gameObject.GetComponent<ToggleMenu>().canBeOpened = false;
                    respawnPanel.SetActive(true);
                }
            }
        }

        // Reactivates control of player and removes death panel
        public void Respawn()
        {
            if (respawnPanel.activeInHierarchy)
            {
                gameObject.GetComponent<ToggleCharacterScreen>().locked = false;
                gameObject.GetComponent<PlayerScript3D>().enabled = true;
                gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = true;
                gameObject.GetComponent<ToggleMenu>().canBeOpened = true;
                respawnPanel.SetActive(false);
            }
        }
    }
}