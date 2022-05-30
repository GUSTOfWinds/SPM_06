using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby Instance;
    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    
    //Variables
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManagerLobby manager;
    
    //Gameobjects
    public GameObject hostButton;
    //public Text LobbyNameText;

    private void Start()
    {
        if (!SteamManager.Initialized) return;
        if (Instance == null) Instance = this;

        
        manager = GetComponent<NetworkManagerLobby>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }

    public void HostLobby()
    {
        Debug.Log("Initialized steammanager: " + SteamManager.Initialized);
        Debug.Log("Hosting lobby");

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            return;
        }
        hostButton.SetActive(false);
        Debug.Log("Lobby Created");
        
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            SteamFriends.GetPersonaName() + "'s lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining Requested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Called for everyone
        hostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        Debug.Log("Joining lobby");

        //Called on clients
        if (NetworkServer.active) return;
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
        hostButton.SetActive(false);
    }

}
