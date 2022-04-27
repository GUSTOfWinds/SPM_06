using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Mirror;
using UnityEngine;

namespace Event
{
    public class DeathListener : NetworkBehaviour
    {
        private Guid DeathEventGuid;
        private void Start()
        {
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref DeathEventGuid);
        }



        void OnUnitDied(UnitDeathEventInfo unitDeathEventInfo)
        {
            StartCoroutine(DestroyEnemy(unitDeathEventInfo));

        }
        
        IEnumerator DestroyEnemy(UnitDeathEventInfo unitDeathEventInfo)
        {
            float timer = unitDeathEventInfo.RespawnTimer;
            
            
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject p in players) {
                p.GetComponent<PlayerActivateEnemyHealthBar>().RemoveHealthBarAtDeath(unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId);
            }

            var parent = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetRespawnParent();
            
            NetworkServer.Destroy(unitDeathEventInfo.EventUnitGo);
            
            yield return new WaitForSeconds(timer);
            
            // respawn
            parent.GetComponent<EnemySpawner>().EnemySpawn();
        }
    }

}
