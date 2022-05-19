using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class RespawnPanelHandler : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
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
                StartCoroutine(LockPlayer());

                respawnPanel.SetActive(true);
            }

            RpcOnPlayerDeath(playerDeathEventInfo);
        }
    }

    IEnumerator LockPlayer()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<ToggleCharacterScreen>().locked = true;
        gameObject.GetComponent<PlayerScript3D>().enabled = false;
        gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = false;
        gameObject.GetComponent<ToggleMenu>().canBeOpened = false;
    }

    [ClientRpc]
    void RpcOnPlayerDeath(PlayerDeathEventInfo playerDeathEventInfo)
    {
        if (isServer || !isLocalPlayer) return;
        {
            if (playerDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId == netID)
            {
                StartCoroutine(LockPlayer());
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