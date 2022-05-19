using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class BreakableSpawner : NetworkBehaviour
{
   
    [Header("Drag the prefab you want to spawn in this spawner here")]
    [SerializeField] private GameObject enemyPrefabToSpawn;
    [SerializeField] private MeshRenderer meshRenderer;
    private Guid respawnEventGuid;
    private uint netID;
    private void Awake()
    {
        // Registers a listener for respawnevents
        EventSystem.Current.RegisterListener<BreakableRespawnEventInfo>(BreakableRespawn, ref respawnEventGuid);
    }

    private void Start()
    {
        // Caches the netID
        netID = gameObject.GetComponent<NetworkIdentity>().netId;
    }
    public void BreakableRespawn(BreakableRespawnEventInfo breakableRespawnEventInfo)
    {
        if (breakableRespawnEventInfo.respawnParent.GetComponent<NetworkIdentity>().netId == netID)
        {
            Spawn();
        }
    }
    public void Spawn()
    {

        // Will be changed to happen ONCE when event manager handles deaths.
        // Spawns an enemy at the location of the spawner parent, will also spawn it on the server
        var enemy = Instantiate(enemyPrefabToSpawn, gameObject.transform.position, Quaternion.identity, null);
        enemy.GetComponent<BreakableInfo>().SetRespawnAnchor(transform);
        NetworkServer.Spawn(enemy);
        meshRenderer.enabled = true;

    }

    public override void OnStartServer()
    {
        Spawn();
    }
}