using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace ItemNamespace
{
    /**
     * @author Martin Kings
     */
    public class PlayerTeleport : NetworkBehaviour
    {
        [SerializeField] private GameObject teleportSpot;
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private Button button;
        [SyncVar] [SerializeField] public bool clickedYes;
        private bool allClickedYes;
        private GameObject[] players;
        private Vector3 portPosition;
        private ToggleCharacterScreen toggleCharacterScreen;
        private PlayerScript3D playerScript3D;
        private CameraMovement3D cameraMovement3D;

        private void Start()
        {
            teleportSpot = GameObject.FindGameObjectWithTag("BossPortal");
            toggleCharacterScreen = gameObject.GetComponent<ToggleCharacterScreen>();
            playerScript3D = gameObject.GetComponent<PlayerScript3D>();
            cameraMovement3D = gameObject.GetComponent<CameraMovement3D>();
            portPosition = teleportSpot.transform.position;
        }

        // Opens up the confirmation panel for both players.
        public void StartTeleport()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (isServer)
            {
                RpcStartConfirmation();
            }
            else
            {
                uiPanel.SetActive(true);
                toggleCharacterScreen.locked = true;
                playerScript3D.enabled = false;
                cameraMovement3D.shouldBeLocked = false;

                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                CmdStartConfirmation();
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdStartConfirmation()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            uiPanel.SetActive(true);
            toggleCharacterScreen.locked = true;
            playerScript3D.enabled = false;
            cameraMovement3D.shouldBeLocked = false;

            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }

        [ClientRpc]
        private void RpcStartConfirmation()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            uiPanel.SetActive(true);
            toggleCharacterScreen.locked = true;
            playerScript3D.enabled = false;
            cameraMovement3D.shouldBeLocked = false;

            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }

        // Called upon by the Yes button under the panel UI gameobject
        public void YesButtonPressed()
        {
            // Returns if the script runs on an object that isn't the local player
            if (!isLocalPlayer)
            {
                return;
            }

            if (isServer) // Runs Rpc on both host and clients
            {
                RpcYesButtonPressed();
            }
            else // Runs on THIS client first, then host
            {
                clickedYes = true;
                players = GameObject.FindGameObjectsWithTag("Player");
                allClickedYes = true; // will be replaced by false if any players hasn't clicked yes
                foreach (var player in players)
                {
                    if (player.GetComponent<PlayerTeleport>().clickedYes == false)
                    {
                        allClickedYes = false;
                        break;
                    }
                }

                // if all players have pressed yes in the popup, the players will be teleported and the
                // pop up will be disabled
                if (allClickedYes)
                {
                    foreach (var player in players)
                    {
                        player.GetComponent<PlayerTeleport>().CmdRemoveConfirmationScreen();
                        player.transform.position = portPosition;
                    }
                }
                else
                {
                    button.enabled = false;
                }

                CmdYesButtonPressed();
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdYesButtonPressed()
        {
            clickedYes = true;
            players = GameObject.FindGameObjectsWithTag("Player");
            allClickedYes = true; // will be replaced by false if any players hasn't clicked yes
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerTeleport>().clickedYes == false)
                {
                    allClickedYes = false;
                    break;
                }
            }

            if (allClickedYes)
            {
                // Resets to default so boss can be reached again
                clickedYes = false;
                button.enabled = true;
                // Sets the Panel and script state to active
                uiPanel.SetActive(false);
                toggleCharacterScreen.locked = false;
                playerScript3D.enabled = true;
                cameraMovement3D.shouldBeLocked = true;

                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                foreach (var player in players)
                {
                    player.GetComponent<PlayerTeleport>().CmdRemoveConfirmationScreen();
                    player.transform.position = portPosition;
                }
            }
            else
            {
                button.enabled = false;
            }
        }

        [ClientRpc]
        private void RpcYesButtonPressed()
        {
            clickedYes = true;
            players = GameObject.FindGameObjectsWithTag("Player");
            allClickedYes = true; // will be replaced by false if any players hasn't clicked yes
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerTeleport>().clickedYes == false)
                {
                    allClickedYes = false;
                    break;
                }
            }

            if (allClickedYes)
            {
                // Resets to default so boss can be reached again
                clickedYes = false;
                button.enabled = true;
                // Sets the Panel and script state to active
                uiPanel.SetActive(false);
                toggleCharacterScreen.locked = false;
                playerScript3D.enabled = true;
                cameraMovement3D.shouldBeLocked = true;

                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;

                foreach (var player in players)
                {
                    if (isServer)
                    {
                        player.GetComponent<PlayerTeleport>().RpcRemoveConfirmationScreen();
                    }

                    player.transform.position = portPosition;
                }
            }
            else
            {
                button.enabled = false;
            }
        }

        [ClientRpc]
        private void RpcRemoveConfirmationScreen()
        {
            // Returns if the script runs on an object that isn't the local player
            if (!isLocalPlayer)
            {
                return;
            }

            // Resets to default so boss can be reached again
            clickedYes = false;
            button.enabled = true;
            // Sets the Panel and script state to active
            uiPanel.SetActive(false);
            toggleCharacterScreen.locked = false;
            playerScript3D.enabled = true;
            cameraMovement3D.shouldBeLocked = true;

            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

        [Command(requiresAuthority = false)]
        private void CmdRemoveConfirmationScreen()
        {
            // Returns if the script runs on an object that isn't the local player
            if (!isLocalPlayer)
            {
                return;
            }

            // Resets to default so boss can be reached again
            clickedYes = false;
            button.enabled = true;
            // Sets the Panel and script state to active
            uiPanel.SetActive(false);
            toggleCharacterScreen.locked = false;
            playerScript3D.enabled = true;
            cameraMovement3D.shouldBeLocked = true;

            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Called upon by the No button under the UI panel Gameobject
        public void NoButtonPressed()
        {
            // Returns if the script runs on an object that isn't the local player
            if (!isLocalPlayer)
            {
                return;
            }

            players = GameObject.FindGameObjectsWithTag("Player");

            if (isServer)
            {
                foreach (var player in players)
                {
                    player.GetComponent<PlayerTeleport>().RpcRemoveConfirmationScreen();
                }
            }
            else
            {
                // Resets to default so boss can be reached again
                clickedYes = false;
                button.enabled = true;
                // Sets the Panel and script state to active
                uiPanel.SetActive(false);
                toggleCharacterScreen.locked = false;
                playerScript3D.enabled = true;
                cameraMovement3D.shouldBeLocked = true;

                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                foreach (var player in players)
                {
                    player.GetComponent<PlayerTeleport>().CmdRemoveConfirmationScreen();
                }
            }
        }
    }
}