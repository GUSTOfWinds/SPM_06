using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ToggleMenu : NetworkBehaviour
{
    [SerializeField] private GameObject menuScreen;

    public void ToggleScreen()
    {
        if (base.isServer)
        {
            // Runs the method below on all clients
            RpcLockScreen();
        }
    }

    [ClientRpc]
    public void RpcLockScreen()
    {
        if (menuScreen.active)
        {
            Cursor.lockState = CursorLockMode.Locked;
            menuScreen.SetActive(false);
            this.GetComponent<PlayerScript3D>().enabled = true;
            this.GetComponent<CameraMovement3D>().enabled = true;
            this.GetComponent<ToggleCharacterScreen>().enabled = true;
            Time.timeScale = 1;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            menuScreen.SetActive(true);
            this.GetComponent<PlayerScript3D>().enabled = false;
            this.GetComponent<CameraMovement3D>().enabled = false;
            this.GetComponent<ToggleCharacterScreen>().enabled = false;
            Time.timeScale = 0;
        }
    }
}