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
        

        [SerializeField] private GameObject[] objects;
        private Guid breakEventGuid;
        private Guid respawnObjectEventGuid;
        private BreakableInfo breakableInfo;


        private void Start()
        {
            StartCoroutine(FetchInitialEnemies());
            EventSystem.Current.RegisterListener<BreakableDestroyedEventInfo>(OnUnitDied, ref breakEventGuid);
            EventSystem.Current.RegisterListener<BreakableRespawnEventInfo>(OnUnitRespawn, ref respawnObjectEventGuid);
        }

        // Will at start get all enemies in the scene
        private IEnumerator FetchInitialEnemies()
        {
            yield return new WaitForSeconds(1);
            objects = GameObject.FindGameObjectsWithTag("Breakable");
        }
        // Will refresh the array of all enemies if an enemy respawns

        void OnUnitRespawn(BreakableRespawnEventInfo breakableDestroyedEventInfo)
        {
            objects = GameObject.FindGameObjectsWithTag("Breakable");
        }

        // Will refresh the array of all enemies if an enemy dies and then perform
        // all death sequences for the enemy that has died
        void OnUnitDied(BreakableDestroyedEventInfo breakableDestroyedEventInfo)
        {
            RefreshEnemyArrays(breakableDestroyedEventInfo);
            StartCoroutine(DestroyEnemy(breakableDestroyedEventInfo));
        }


        IEnumerator DestroyEnemy(BreakableDestroyedEventInfo breakableDestroyedEventInfo)
        {
            if (isServer)
            {
                float timer = breakableDestroyedEventInfo.RespawnTimer;
                // Fetches the enemyinfo script from the enemy
                breakableInfo = breakableDestroyedEventInfo.EventUnitGo.transform.GetComponent<BreakableInfo>();

                var parent = breakableInfo.GetRespawnParent();

                // Randomizes a number between 1 and the dropchance int set in character base for drops, if
                // it is a match, the drop will appear
              

                // Destroys the enemy
                NetworkServer.Destroy(breakableDestroyedEventInfo.EventUnitGo);

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

        private void RefreshEnemyArrays(BreakableDestroyedEventInfo breakableDestroyedEventInfo)
        {
            objects = GameObject.FindGameObjectsWithTag("Breakable");
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    if (objects[i].GetComponent<NetworkIdentity>().netId ==
                        breakableDestroyedEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId)
                    {
                        objects[i] = null;
                    }
                }
            }
        }

        public GameObject[] GetEnemies()
        {
            return objects;
        }
    }
}
