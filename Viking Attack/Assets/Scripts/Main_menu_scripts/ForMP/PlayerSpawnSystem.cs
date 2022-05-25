using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Main_menu_scripts.ForMP
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab = null;

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

        public void SpawnPlayer(NetworkConnectionToClient conn)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);


            if (spawnPoint != null)
            {
                GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
                NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
            }


            nextIndex++;
        }
    }
}