using System;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;

public class NewEnemySpawner : NetworkBehaviour
{
    
    [Header("Drag the prefab you want to spawn in this spawner here")]
    [SerializeField] private GameObject enemyPrefabToSpawn;
    private Guid respawnEventGuid;
    private uint netID;
    [SerializeField] private bool isBoss;
    [SerializeField] private bool roaming;
    [SerializeField] private float roamingRange = 20;
    [SerializeField] private float aggroRange = 30;
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private int dropChance;

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
        /*
            @Author Love Strignert - lost9373
        */
        // Spawns an enemy at the location of the spawner and set that enemys veribals, will also spawn it on the server
        var enemy = Instantiate(enemyPrefabToSpawn, gameObject.transform.position, gameObject.transform.rotation, null);
        enemy.GetComponent<EnemyInfo>().SetRespawnAnchor(transform);
        enemy.GetComponent<EnemyInfo>().SetDropItem(itemDrop);
        enemy.GetComponent<EnemyInfo>().SetDropChance(dropChance);
        enemy.GetComponent<EnemyAIScript>().SetEnemyTransform(transform);
        enemy.GetComponent<EnemyAIScript>().SetIfEnemyRoam(roaming);
        enemy.GetComponent<EnemyAIScript>().SetAggroRange(aggroRange);
        enemy.GetComponent<EnemyAIScript>().SetRoamingRange(roamingRange);
        NetworkServer.Spawn(enemy);
        

    }

    public override void OnStartServer()
    {
        Spawn();
    }
}
