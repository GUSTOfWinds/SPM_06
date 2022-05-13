using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class EnemySpawner : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [Header("Drag the prefab you want to spawn in this spawner here")]
    [SerializeField] private GameObject enemyPrefabToSpawn;
    private Guid respawnEventGuid;
    private uint netID;
    [SerializeField] private bool isBoss;


    private void Awake()
    {
        // Registers a listener for respawnevents
        EventSystem.Current.RegisterListener<EnemyRespawnEventInfo>(EnemyRespawn, ref respawnEventGuid);
    }

    private void Start()
    {
        // Caches the netID
        netID = gameObject.GetComponent<NetworkIdentity>().netId;
    }

    // Will be run whenever a respawn has been detected by the event system
    public void EnemyRespawn(EnemyRespawnEventInfo enemyRespawnEventInfo)
    {
        if (enemyRespawnEventInfo.respawnParent.GetComponent<NetworkIdentity>().netId == netID && !isBoss)
        {
            Spawn();
        }
    }

    public void Spawn()
    {

    // Will be changed to happen ONCE when event manager handles deaths.
        // Spawns an enemy at the location of the spawner parent, will also spawn it on the server
        var enemy = Instantiate(enemyPrefabToSpawn, gameObject.transform.position, Quaternion.identity, null);
        enemy.GetComponent<EnemyInfo>().SetRespawnAnchor(transform);
        enemy.GetComponent<EnemyMovement>().SetEnemyTransform(transform);
        NetworkServer.Spawn(enemy);
        

    }

    public override void OnStartServer()
    {
        Spawn();
    }
}
