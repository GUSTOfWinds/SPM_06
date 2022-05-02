using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    private int minPlayers;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    
    [Header("Room")] 
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerLobby;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {

        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
 
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            
            
            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerLobby);

            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerLobby.gameObject);
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            
            player.HandleReadyToStart(IsReadyTostart());
        }
    }

    private bool IsReadyTostart()
    {
        //TODO SÄTT COUNTDOWN OM HÄLFTEN SÄGER ATT DE ÄR KLARA
        int ready = 0;
        if (numPlayers <= 0) return false; 
        foreach (var player in RoomPlayers)
        {
            if (player.isReady)
            {
                ready++;
            }
            
        }
        if (ready >= numPlayers / 2)
        {
            return true;
        }

        return false;

    }
}