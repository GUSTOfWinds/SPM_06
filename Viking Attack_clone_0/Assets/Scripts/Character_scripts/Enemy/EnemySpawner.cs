using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Drag the prefab you want to spawn in this spawner here")]
    [SerializeField] private GameObject enemyPrefabToSpawn;
    [SerializeField] private bool stop;
    
    public void EnemySpawn()
    {
        // Will be changed to happen ONCE when event manager handles deaths.
        // Spawns an enemy at the location of the spawner parent, will also spawn it on the server
            var parentGO = gameObject;
            var enemy = Instantiate(enemyPrefabToSpawn, parentGO.transform.position, parentGO.transform.rotation, null);
            NetworkServer.Spawn(enemy);

    }

    public override void OnStartServer()
    {
        
        EnemySpawn();
    }
}
