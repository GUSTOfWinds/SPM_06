using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Main_menu_scripts.ForMP
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        //[SerializeField] private GameObject playerPrefab = null;
        public List<GameObject> PlayerList { get; set; } = new List<GameObject>();
        private static List<Transform> spawnPoints = new List<Transform>();
        
        private int nextIndex = 0;

        public static void AddSpawnPoint(Transform transform) 
        {
            spawnPoints.Add(transform);

            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }
        public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

        public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

        [ServerCallback]
        private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

        public void SpawnPlayer(NetworkConnectionToClient conn, List<GameObject> players)
        {
            PlayerList = players;

            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPoints[i].transform.position;
                players[i].transform.rotation = spawnPoints[i].transform.rotation;
                NetworkServer.Spawn(players[i]);

            }
        }
    }
}