using System;
using System.Collections;
using ItemNamespace;
using UnityEngine;
using Mirror;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Event
{
    public class DeathListener : NetworkBehaviour
    {
        [SerializeField] private GameObject dropBase;
        [SerializeField] private GameObject dropDataBase;
        [SerializeField] private GameObject[] enemies;
        private Guid deathEventGuid;
        private Guid respawnEventGuid;


        private void Start()
        {
            StartCoroutine(FetchInitialEnemies());
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref deathEventGuid);
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
            if (isServer)
            {
                float timer = unitDeathEventInfo.RespawnTimer;
                uint netIDOfEnemy = unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId;
                var parent = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetRespawnParent();

                // Randomizes a number between 1 and the dropchance int set in character base for drops, if
                // it is a match, the drop will appear
                int randomMax = unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetDropChance() + 1;

                if (unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop() != null)
                {
                    if (Random.Range(1, randomMax) ==
                        unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetDropChance())
                    {
                        if (dropDataBase.GetComponent<DropDatabase>().GetIsDropped(unitDeathEventInfo.EventUnitGo
                                .transform
                                .GetComponent<EnemyInfo>().GetDrop().GetComponent<DropItemInWorldScript>().itembase) ==
                            false)
                        {
                            EventInfo itemDropEventInfo = new ItemDropEventInfo
                            {
                                itemBase = unitDeathEventInfo.EventUnitGo
                                    .transform
                                    .GetComponent<EnemyInfo>().GetDrop().GetComponent<DropItemInWorldScript>().itembase
                            };
                            EventSystem.Current.FireEvent(itemDropEventInfo);


                            // instantiates the drop that has been added to the enemy prefab and networkspawns it

                            var drop = Instantiate(
                                unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>().GetDrop(),
                                new Vector3(unitDeathEventInfo.EventUnitGo.transform.position.x,
                                    unitDeathEventInfo.EventUnitGo.transform.position.y + 1,
                                    unitDeathEventInfo.EventUnitGo.transform.position.z), new Quaternion(0, 0, 0, 0));
                            NetworkServer.Spawn(drop);
                        }
                    }
                }

                // Destroys the enemy
                NetworkServer.Destroy(unitDeathEventInfo.EventUnitGo);

                // Destroys the health bars
                yield return new WaitForSeconds(timer);

                // Respawns the enemy at the same spawner
                RespawnEnemy(parent);
            }
        }

        private void RespawnEnemy(Transform respawnParent)
        {
            // Sets up a respawn event
            EventInfo unitRespawnInfo = new EnemyRespawnEventInfo
            {
                respawnParent = respawnParent
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
                    if (enemies[i].GetComponent<NetworkIdentity>().netId ==
                        unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId)
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