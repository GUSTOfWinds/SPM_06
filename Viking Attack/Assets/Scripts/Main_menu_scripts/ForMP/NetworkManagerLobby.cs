using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main_menu_scripts.ForMP
{
    public class NetworkManagerLobby : NetworkManager
    {
        private int minPlayers = 1;
        [Scene] [SerializeField] private string menuScene = string.Empty;
    
        [Header("Room")] 
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerLobby;
        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

        [Header("Game")] 
        [SerializeField] private NetworkGamePlayer gamePlayerPrefab;

        [SerializeField] private GameObject playerSpawnSystem;
        public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();
        public List<GameObject> spawnablePrefabs = new List<GameObject>();

    
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnectionToClient> OnServerReadied; 




        public override void OnStartServer()
        {
            spawnPrefabs = spawnablePrefabs;
            base.OnStartServer();
        }

        public override void OnStartClient()
        {
            var prefabs = this.spawnablePrefabs;
            foreach (var prefab in prefabs)
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

            if (SceneManager.GetActiveScene().path != menuScene)
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
            base.OnServerDisconnect(conn);
 
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;
            
            
                NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerLobby);

                roomPlayerInstance.IsLeader = isLeader;
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
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
            int ready = 0;
            if (numPlayers <= 0) return false; 
            foreach (var player in RoomPlayers)
            {
                if (player.isReady)
                {
                    ready++;
                }
            
            }
            if (ready >= numPlayers )
            {
                return true;
            }

            return false;
        }

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsReadyTostart()) return;
                ServerChangeScene("TerrainIsland"); //TODO bestäm vilken start-kartan är
            }
        }


        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Scene_Map"))
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                    //NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }
            }

            base.ServerChangeScene(newSceneName);
        }


        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            OnServerReadied?.Invoke(conn);
        }
    

        public override void OnServerSceneChanged(String sceneName)
        {
            //if (!sceneName.Contains("Scene_Map")) return;
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }
}