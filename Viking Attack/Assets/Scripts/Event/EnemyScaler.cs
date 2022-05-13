using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;

public class EnemyScaler : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private Guid deathEventGuid;
    private Guid respawnEventGuid;
    private Guid playerConnectEventGuid;

    void Start()
    {
        EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref deathEventGuid);
        EventSystem.Current.RegisterListener<EnemyRespawnEventInfo>(OnUnitRespawn, ref respawnEventGuid);
        EventSystem.Current.RegisterListener<PlayerConnectEventInfo>(OnPlayerJoin, ref playerConnectEventGuid);
        StartCoroutine(FetchInitialEnemies());
    }


    // Runs at start, will fetch all enemies that start in the scene
    private IEnumerator FetchInitialEnemies()
    {
        yield return new WaitForSeconds(1);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Any time an enemy respawns, the array of enemies will be refreshed
    void OnUnitRespawn(EnemyRespawnEventInfo unitDeathEventInfo)
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<EnemyInfo>().PlayerScale();
        }
    }

    // Will be run any time a player joins and scales the damage and health
    // in each enemyinfo script available.
    void OnPlayerJoin(PlayerConnectEventInfo playerConnectEventInfo)
    {
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<EnemyInfo>().PlayerScale();
        }
    }

    // The enemy will be removed from the array when it dies
    void OnUnitDied(UnitDeathEventInfo unitDeathEventInfo)
    {
        RefreshEnemyArrays(unitDeathEventInfo);
    }

    // Removes an enemy from the array
    private void RefreshEnemyArrays(UnitDeathEventInfo unitDeathEventInfo)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                if (enemies[i].GetComponent<NetworkIdentity>().netId ==
                    unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId)
                {
                    enemies[i] = null;
                }
            }
        }
    }
}