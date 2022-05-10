﻿using System;
using Mirror;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [Header("Drag the item prefab you want to spawn in this spawner here")] [SerializeField]
    private GameObject enemyPrefabToSpawn;

    private uint netID;
    [SerializeField] private bool spawnAtServerStart;

    public void Spawn()
    {
        // Spawns an item at the location of the spawner parent
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