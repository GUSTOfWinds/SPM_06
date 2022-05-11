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

        [Server]
        public void SpawnPlayer(NetworkConnectionToClient conn)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
            Debug.Log("Position"+spawnPoint.position);
            Debug.Log("Rotation"+spawnPoint.rotation);

            if (spawnPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }
            

            //NetworkServer.Destroy(conn.identity.gameObject);

            GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
            //NetworkServer.Spawn(conn);
            NetworkServer.ReplacePlayerForConnection(conn, playerInstance);
            NetworkServer.Spawn(playerInstance, conn);
            
            nextIndex++;
        }
    }
}