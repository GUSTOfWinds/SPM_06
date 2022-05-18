using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ItemNamespace;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;

namespace Event
{
    public class DeathListener : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private GameObject dropDataBase;

        [SerializeField] private GameObject[] enemies;
        private Guid deathEventGuid;
        private Guid respawnEventGuid;
        private EnemyInfo enemyInfo;


        private void Start()
        {
            StartCoroutine(FetchInitialEnemies());
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref deathEventGuid);
            EventSystem.Current.RegisterListener<EnemyRespawnEventInfo>(OnUnitRespawn, ref respawnEventGuid);
        }

        // Will at start get all enemies in the scene
        private IEnumerator FetchInitialEnemies()
        {
            yield return new WaitForSeconds(1);
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        // Will refresh the array of all enemies if an enemy respawns

        void OnUnitRespawn(EnemyRespawnEventInfo unitDeathEventInfo)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        // Will refresh the array of all enemies if an enemy dies and then perform
        // all death sequences for the enemy that has died
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
                // Fetches the enemyinfo script from the enemy
                enemyInfo = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>();

                var parent = enemyInfo.GetRespawnParent();

                // Randomizes a number between 1 and the dropchance int set in character base for drops, if
                // it is a match, the drop will appear
                int randomMax = enemyInfo.GetDropChance() + 1;

                if (enemyInfo.GetDrop() != null)
                {
                    if (Random.Range(1, randomMax) ==
                        enemyInfo.GetDropChance())
                    {
                        if (dropDataBase.GetComponent<DropDatabase>()
                                .GetIsDropped(enemyInfo.GetDrop().GetComponent<DropItemInWorldScript>().itembase) ==
                            false)
                        {
                            EventInfo itemDropEventInfo = new ItemDropEventInfo
                            {
                                itemBase = enemyInfo.GetDrop().GetComponent<DropItemInWorldScript>().itembase
                            };
                            EventSystem.Current.FireEvent(itemDropEventInfo);


                            // instantiates the drop that has been added to the enemy prefab and networkspawns it

                            var drop = Instantiate(
                                enemyInfo.GetDrop(),
                                new Vector3(unitDeathEventInfo.EventUnitGo.transform.position.x,
                                    unitDeathEventInfo.EventUnitGo.transform.position.y + 1,
                                    unitDeathEventInfo.EventUnitGo.transform.position.z),
                                new Quaternion(0, 0, 0, 0));
                            NetworkServer.Spawn(drop);
                        }
                    }
                }

                // Destroys the enemy
                NetworkServer.Destroy(unitDeathEventInfo.EventUnitGo);

                yield return new WaitForSeconds(timer);

                // Respawns the enemy at the same spawner after the timer that has been set in event info
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