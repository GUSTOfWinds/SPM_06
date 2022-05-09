using System;
using System.Collections;
using ItemNamespace;
using UnityEngine;
using Mirror;
namespace Event
{
    public class DeathListener : MonoBehaviour
    {
        [SerializeField] private GameObject dropBase;
        [SerializeField] private GameObject[] enemies;
        private Guid DeathEventGuid;
        private Guid respawnEventGuid;
        
        private void Start()
        {
            StartCoroutine(FetchInitialEnemies());
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref DeathEventGuid);
            EventSystem.Current.RegisterListener<EnemyRespawnEventInfo>(OnUnitRespawn, ref respawnEventGuid);
            
        }

        private IEnumerator FetchInitialEnemies()
        {
            yield return new WaitForSeconds(1);
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        void OnUnitRespawn(EnemyRespawnEventInfo unitDeathEventInfo)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        void OnUnitDied(UnitDeathEventInfo unitDeathEventInfo)
        {
            RefreshEnemyArrays(unitDeathEventInfo);
            StartCoroutine(DestroyEnemy(unitDeathEventInfo));
        }

        
        IEnumerator DestroyEnemy(UnitDeathEventInfo unitDeathEventInfo)
        {
            float timer = unitDeathEventInfo.RespawnTimer;
            uint netIDOfEnemy = unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId;
            var parent = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetRespawnParent();
            

            dropBase.GetComponent<DropItemInWorldScript>().itembase = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop();
            if(unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop() != null)
            {
                var drop = Instantiate(dropBase,new Vector3(unitDeathEventInfo.EventUnitGo.transform.position.x,unitDeathEventInfo.EventUnitGo.transform.position.y + 1,unitDeathEventInfo.EventUnitGo.transform.position.z),new Quaternion(0,0,0,0));
                NetworkServer.Spawn(drop);
            }
            
            // Destroys the enemy
            NetworkServer.Destroy(unitDeathEventInfo.EventUnitGo);

            // Destroys the health bars
            yield return new WaitForSeconds(timer);
            
            // Respawns the enemy at the same spawner
            RespawnEnemy(parent);
        }

        private void RespawnEnemy(Transform respawnParent)
        {
            
            // Sets up a respawn event
            EventInfo unitRespawnInfo = new EnemyRespawnEventInfo
            {
                parent = respawnParent
            };
            
            // Ships the respawn event
            EventSystem.Current.FireEvent(unitRespawnInfo);
        }

        private void RefreshEnemyArrays(UnitDeathEventInfo unitDeathEventInfo)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    if (enemies[i].GetComponent<NetworkIdentity>().netId == unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId)
                    {
                        enemies[i] = null;
                    }
                }
            }
        }

        public GameObject[] GetEnemies()
        {
            return enemies;
        }
    }
}
