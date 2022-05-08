using System;
using System.Collections;
using ItemNamespace;
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
            uint netIDOfEnemy = unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId;
            var parent = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetRespawnParent();
            
            // Destroys the enemy
            NetworkServer.Destroy(unitDeathEventInfo.EventUnitGo);
            
            // Destroys the health bars
            yield return new WaitForSeconds(timer);
            
            // respawn
            parent.GetComponent<EnemySpawner>().EnemySpawn();
        }
    }

}
