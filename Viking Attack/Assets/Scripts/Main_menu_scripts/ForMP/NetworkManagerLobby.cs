using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
 * @author Victor Wikner
 */
public class NetworkManagerLobby : NetworkManager
{
    //private int minPlayers = 1;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")] [SerializeField] private NetworkRoomPlayerLobby roomPlayerLobby;
    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

    [Header("Game")] [SerializeField] private NetworkGamePlayer gamePlayerPrefab;

    [SerializeField] private GameObject playerSpawnSystem;
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();
    public List<GameObject> InGamePlayer { get; } = new List<GameObject>();
    public List<GameObject> spawnablePrefabs = new List<GameObject>();

    [SerializeField] private GameObject landingPage;
    [Header("Scene")] [SerializeField] public string mapToLoad;
    [SerializeField] private GameObject playerPrefabFinalUse;


    private bool SceneChanged;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnectionToClient, List<GameObject>> OnServerReadied;



    private void Start()
    {
        spawnPrefabs = spawnablePrefabs;
    }


    public override void OnStartServer()
    {
        spawnPrefabs = spawnablePrefabs;
        base.OnStartServer();
        NetworkServer.RegisterHandler<CharacterInfo>(OnSpawnPlayerUI);
    }

    //Sets spawnable prefabs when a client is started
    public override void OnStartClient()
    {
        var prefabs = spawnablePrefabs;
        foreach (var prefab in prefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    //Sets customizable data in a message and sends to the server.
    public override void OnClientConnect(NetworkConnection conn)
    {
        byte r = (byte)PlayerPrefs.GetInt("redValue");
        byte g = (byte)PlayerPrefs.GetInt("greenValue");
        byte b = (byte)PlayerPrefs.GetInt("blueValue");
        Color32 color = new Color32(r, g, b, 255);
        base.OnClientConnect(conn);
        CharacterInfo characterInfo = new CharacterInfo
        {
            playerColour = color,
            name = PlayerPrefs.GetString("PlayerName")
        };
        conn.Send(characterInfo);

        OnClientConnected?.Invoke();
        if (SceneChanged == true)
        {
            ServerChangeScene(mapToLoad);
        }
    }

    //removes client on disconnect
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        SceneManager.LoadScene(0);


        OnClientDisconnected?.Invoke();
    }
    //adds client on connect with exceptions of overfill 
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        /*
        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
        }*/
    }

    //Removes a player from all clients when disconnecting
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            var player2 = conn.identity.GetComponent<NetworkGamePlayer>();
            var player3 = conn.identity.GetComponent<GameObject>();

            GamePlayers.Remove(player2);
            RoomPlayers.Remove(player);
            InGamePlayer.Remove(player3);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    //Adds a player who joins into the lobby and shows to all players.
    public void OnSpawnPlayerUI(NetworkConnectionToClient conn, CharacterInfo info)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerLobby);

            roomPlayerInstance.IsLeader = isLeader;
            roomPlayerInstance.name = info.name;
            roomPlayerInstance.colour = info.playerColour;

            if (roomPlayerInstance.hasAuthority)
            {
                roomPlayerInstance.gameObject.SetActive(false);
            }

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    //Removes all players from lists when server is stopped and returns to first scene    
    public override void OnStopServer()
    {
        SceneManager.LoadScene(0);
        if (landingPage != null) landingPage.SetActive(true);

        RoomPlayers.Clear();
        GamePlayers.Clear();
        InGamePlayer.Clear();
    }

    //Updates what the ready status is of all players in the lobby
    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    //checks if ready players is more or less than size of the amount of connected players
    private bool IsReadyToStart()
    {
        int ready = 0;
        if (numPlayers <= 0) return false;
        foreach (var player in RoomPlayers)
        {
            if (player.isReady)
            {
                ready++;
            }
        }

        if (ready >= numPlayers)
        {
            return true;
        }

        return false;
    }

    //starts scene and invokes a scene-change for all players. if all players aren't ready to start we ignore the call,
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) return;
            SceneChanged = true;
            ServerChangeScene(mapToLoad);
        }
    }


    //Changes the scene using name of scene sent from other method, creates object to bring data into the game
    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Terr"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].displayName);
                gamePlayerInstance.SetSkinColour(RoomPlayers[i].colour);

                //NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                GamePlayers.Add(gamePlayerInstance);
            }
        }


        base.ServerChangeScene(newSceneName);
    }


    //checks if the server is ready for players to join the new scene, and adds players if it is
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn, InGamePlayer);

    }



    //Method is called when scene has changed, it instantiates the players then spawns the spawn-system which sets position, rotation and spawns the players, i.e makes sure they're all
    [Server]
    public override void OnServerSceneChanged(String sceneName)
    {
        //if (!sceneName.Contains("Scene_Map")) return;
        var conn = GamePlayers[0].connectionToClient;

        foreach (var t in GamePlayers)
        {
            conn = t.connectionToClient;
            GameObject playerInGame = Instantiate(playerPrefabFinalUse);
            playerInGame.GetComponent<GlobalPlayerInfo>().SetDisplayName(t.displayName);
            playerInGame.GetComponent<GlobalPlayerInfo>().SetSkinColour(t.colour);
            InGamePlayer.Add(playerInGame);
            NetworkServer.ReplacePlayerForConnection(conn, playerInGame.gameObject, true);
        }
        var playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
        NetworkServer.Spawn(playerSpawnSystemInstance);
    }

    // for loading data By Jiang
    public NetworkRoomPlayerLobby GetLobbyRoom()
    {
        return roomPlayerLobby;
    }
}
/**
 * @author Victor Wikner
 * data to send with the player
 */
public struct CharacterInfo : NetworkMessage
{
    public string name;
    public Color32 playerColour;
}