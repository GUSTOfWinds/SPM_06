using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleMenu : NetworkBehaviour
{
    private GameObject[] players;

    // Called upon from either host or the client, client will however not be able to 
    // pause the game
    public void OpenScreen()
    {
        if (isServer)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                RpcOpenMenu(player);
            }
        }
    }

    // Called upon from either host or client, client will however not be able to 
    // unpause the game
    public void CloseScreen()
    {
        if (isServer)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                RpcCloseMenu(player);
            }
        }
    }
    
    // Closes the menu and unpauses the game
    [ClientRpc]
    private void RpcCloseMenu(GameObject player)
    {
        gameObject.GetComponent<PlayerInput>().enabled = true;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(false);
    }
    
    // Opens the menu and pauses the game
    [ClientRpc]
    private void RpcOpenMenu(GameObject player)
    {
        gameObject.GetComponent<PlayerInput>().enabled = false;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(true);
    }
}