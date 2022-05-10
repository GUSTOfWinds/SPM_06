using System;
using Mirror;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [Header("Drag the prefab you want to spawn in this spawner here")] [SerializeField]
    private GameObject enemyPrefabToSpawn;

    private uint netID;
    [SerializeField] private bool spawnAtServerStart;

    public void Spawn()
    {
        // Will be changed to happen ONCE when event manager handles deaths.
        // Spawns an enemy at the location of the spawner parent, will also spawn it on the server
        var item = Instantiate(enemyPrefabToSpawn, gameObject.transform.position, Quaternion.identity, null);
        NetworkServer.Spawn(item);
    }

    public override void OnStartServer()
    {
        if (spawnAtServerStart)
        {
            Spawn();
        }
    }
}