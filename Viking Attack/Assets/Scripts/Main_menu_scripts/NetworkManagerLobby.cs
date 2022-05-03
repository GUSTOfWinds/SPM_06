using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Main_menu_scripts
{
    public class NetworkManagerLobby : NetworkManager
    {
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")] 
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab;
        
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public override void OnStartServer() =>
            spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

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
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }


    }
}
