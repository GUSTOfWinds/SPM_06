using System;
using System.Collections;
using System.Collections.Generic;
using ItemNamespace;
using Mirror;
using UnityEngine;

namespace Event
{
    public class DeathListener : NetworkBehaviour
    {
        [SerializeField] private GameObject dropBase;
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

            dropBase.GetComponent<DropItemInWorldScript>().itembase = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop();
            if(unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop() != null)
            {
                var drop = Instantiate(dropBase,new Vector3(unitDeathEventInfo.EventUnitGo.transform.position.x,unitDeathEventInfo.EventUnitGo.transform.position.y + 1,unitDeathEventInfo.EventUnitGo.transform.position.z),new Quaternion(0,0,0,0));
                NetworkServer.Spawn(drop);
            }
                
            
            // Destroys the health bars
            yield return new WaitForSeconds(timer);
            
            // respawn
            parent.GetComponent<EnemySpawner>().EnemySpawn();
        }
    }

}
