using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager;
    [Header("UI")] 
    [SerializeField] private GameObject landingPanel;

    [Header("Lobby")] 
    [SerializeField] private GameObject lobby;

    public void HostLobby()
    {
        networkManager.StartHost();
        landingPanel.SetActive(false);
        lobby.SetActive(true);
    }
}
