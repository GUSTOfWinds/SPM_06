using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ItemNamespace;
using UnityEngine;
using Mirror;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Event
{
    public class BreakListener : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */

        [SerializeField] private GameObject[] enemies;
        private Guid deathEventGuid;
        private Guid respawnEventGuid;
        private EnemyInfo enemyInfo;


        private void Start()
        {
            StartCoroutine(FetchInitialEnemies());
            EventSystem.Current.RegisterListener<BreakableDestroyedEventInfo>(OnUnitDied, ref deathEventGuid);
            EventSystem.Current.RegisterListener<BreakableRespawnEventInfo>(OnUnitRespawn, ref respawnEventGuid);
        }

        // Will at start get all enemies in the scene
        private IEnumerator FetchInitialEnemies()
        {
            yield return new WaitForSeconds(1);
            enemies = GameObject.FindGameObjectsWithTag("Breakable");
        }
        // Will refresh the array of all enemies if an enemy respawns

        void OnUnitRespawn(BreakableRespawnEventInfo unitDeathEventInfo)
        {
            enemies = GameObject.FindGameObjectsWithTag("Breakable");
        }

        // Will refresh the array of all enemies if an enemy dies and then perform
        // all death sequences for the enemy that has died
        void OnUnitDied(BreakableDestroyedEventInfo unitDeathEventInfo)
        {
            RefreshEnemyArrays(unitDeathEventInfo);
            StartCoroutine(DestroyEnemy(unitDeathEventInfo));
        }


        IEnumerator DestroyEnemy(BreakableDestroyedEventInfo unitDeathEventInfo)
        {
            if (isServer)
            {
                float timer = unitDeathEventInfo.RespawnTimer;
                // Fetches the enemyinfo script from the enemy
                enemyInfo = unitDeathEventInfo.EventUnitGo.transform.GetComponent<EnemyInfo>();

                var parent = enemyInfo.GetRespawnParent();

                // Randomizes a number between 1 and the dropchance int set in character base for drops, if
                // it is a match, the drop will appear
              

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
            EventInfo unitRespawnInfo = new BreakableRespawnEventInfo
            {
                respawnParent = respawnParent
            };

            // Ships the respawn event
            EventSystem.Current.FireEvent(unitRespawnInfo);
        }

        private void RefreshEnemyArrays(BreakableDestroyedEventInfo unitDeathEventInfo)
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
