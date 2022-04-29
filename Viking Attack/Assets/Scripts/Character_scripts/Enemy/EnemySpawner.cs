using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Mirror;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Drag the prefab you want to spawn in this spawner here")]
    [SerializeField] private GameObject enemyPrefabToSpawn;

    public void EnemySpawn()
    {
        // Will be changed to happen ONCE when event manager handles deaths.
        // Spawns an enemy at the location of the spawner parent, will also spawn it on the server
        var enemy = Instantiate(enemyPrefabToSpawn, gameObject.transform.position, Quaternion.identity, null);
            enemy.GetComponent<EnemyInfo>().SetRespawnAnchor(transform);
            NetworkServer.Spawn(enemy);
    }

    public override void OnStartServer()
    {
        EnemySpawn();
    }
}
