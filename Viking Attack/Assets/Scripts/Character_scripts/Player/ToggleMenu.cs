using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleMenu : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    private GameObject[] players;

    private GameObject[] healthBars;
    private List<GameObject> inactiveBars;
    private bool isOpen;

    private void Awake()
    {
        inactiveBars = new List<GameObject>();
    }

    // Called upon from either host or the client, client will however not be able to 
    // pause the game
    public void OpenScreen()
    {
        if (isServer)
        {
            if (!isOpen)
            {
                isOpen = true;
                // // Finds and sets all healthbars to inactive while being paused
                healthBars = GameObject.FindGameObjectsWithTag("EnemyHealthBar");
                foreach (var healthBar in healthBars)
                {
                    inactiveBars.Add(healthBar);
                    healthBar.SetActive(false);
                }

                players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    RpcOpenMenu(player);
                }
            }
            else
            {
                isOpen = false;
                // Finds and sets all healthbars to active when unpaused
                for (int i = 0; i < inactiveBars.Count; i++)
                {
                    inactiveBars[i].SetActive(true);
                    inactiveBars.Remove(inactiveBars[i]);
                }

                players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    RpcCloseMenu(player);
                }
            }
        }
    }

    // Called upon from either host or client, client will however not be able to 
    // unpause the game
    public void CloseScreen()
    {
        isOpen = false;
        if (isServer)
        {
            isOpen = false;
            // Finds and sets all healthbars to active when unpaused
            for (int i = 0; i < inactiveBars.Count; i++)
            {
                inactiveBars[i].SetActive(true);
                inactiveBars.Remove(inactiveBars[i]);
            }

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
        player.GetComponent<ToggleCharacterScreen>().locked = false;
        player.GetComponent<PlayerScript3D>().enabled = true;
        player.GetComponent<CameraMovement3D>().shouldBeLocked = true;

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(false);
    }

    // Opens the menu and pauses the game
    [ClientRpc]
    private void RpcOpenMenu(GameObject player)
    {
        player.GetComponent<ToggleCharacterScreen>().locked = true;
        player.GetComponent<PlayerScript3D>().enabled = false;
        player.GetComponent<CameraMovement3D>().shouldBeLocked = false;

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(true);
    }

    // Could later be used for both players to be able to pause
    [Command]
    private void CmdCloseMenu(GameObject player)
    {
        player.GetComponent<PlayerInput>().enabled = true;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(false);
    }

    [Command]
    private void CmdOpenMenu(GameObject player)
    {
        player.GetComponent<PlayerInput>().enabled = false;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        player.transform.Find("UI").Find("Menu_screen").gameObject.SetActive(true);
    }
}