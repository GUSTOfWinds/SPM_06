using System;
using System.Collections;
using Character_scripts.Enemy;
using Event;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    public class PlayerActivateEnemyHealthBar : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */
        private Guid hitEventGuid;

        private Guid deathEventGuid;

        private Guid playerDeathEventGuid;
        
        private Guid enemyRetreatEventGuid;

        [SerializeField] private GameObject healthBarInHierarchy;

        private EnemyHealthBar enemyHealthBar;

        private uint netIdOfLastHit;

        private uint netIdOfNewHit;

        private uint playerThatHit;

        private uint netIdOfDeadEnemy;
        

        private void Start()
        {
            netIdOfLastHit = 0;
            enemyHealthBar = healthBarInHierarchy.GetComponent<EnemyHealthBar>();
            EventSystem.Current.RegisterListener<EnemyHitEvent>(SetupHealthBar, ref hitEventGuid);
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnEnemyDeath, ref deathEventGuid);
            EventSystem.Current.RegisterListener<PlayerDeathEventInfo>(OnPlayerDeath, ref playerDeathEventGuid);
            EventSystem.Current.RegisterListener<EnemyRetreatingEventInfo>(OnEnemyRetreating, ref enemyRetreatEventGuid);

            if (isLocalPlayer)
            {
                StartCoroutine(DelayedEnemyScale());
            }
        }

        private void OnEnemyRetreating(EnemyRetreatingEventInfo enemyRetreatingEventInfo)
        {
            if (netId == netIdOfLastHit)
            {
                healthBarInHierarchy.SetActive(false);
                netIdOfLastHit = 0;
            }
        }

        // Runs on both client and host, no need to force it to run on client
        void OnPlayerDeath(PlayerDeathEventInfo playerDeathEventInfo)
        {
            uint netIdOfDeadPlayer = playerDeathEventInfo.playerNetId;
            if (netIdOfDeadPlayer == gameObject.GetComponent<NetworkIdentity>().netId)
            {
                healthBarInHierarchy.SetActive(false);
                netIdOfLastHit = 0;
            }
            
        }


        IEnumerator DelayedEnemyScale()
        {
            yield return new WaitForSeconds(1);
            EventInfo playerConnectEventInfo = new PlayerConnectEventInfo
            {
                EventUnitGo = gameObject,
            };
            EventSystem.Current.FireEvent(playerConnectEventInfo);
        }

        // Will pick up any enemy death event on the server, if the killed enemy was the enemy hit last, the 
        // health bar will be reset and shut down.
        private void OnEnemyDeath(UnitDeathEventInfo unitDeathEventInfo)
        {
            netIdOfDeadEnemy = unitDeathEventInfo.EventUnitGo.GetComponent<NetworkIdentity>().netId;
            if (netIdOfDeadEnemy == netIdOfLastHit)
            {
                healthBarInHierarchy.SetActive(false);
                netIdOfLastHit = 0;
            }
            
            RpcOnEnemyDeath(netIdOfDeadEnemy);
        }

        // Will be called on the client if an enemy dies, if the killed enemy was the last hit enemy, the 
        // health bar will be reset.
        [ClientRpc]
        void RpcOnEnemyDeath(uint netId)
        {
            if (!isClientOnly)
            {
                return;
            }

            if (netId == netIdOfLastHit)
            {
                healthBarInHierarchy.SetActive(false);
                netIdOfLastHit = 0;
            }
        }


        // Sets up the health bar instance and assigns proper values
        private void SetupHealthBar(EnemyHitEvent hit)
        {
            playerThatHit = hit.playerNetId;
            netIdOfNewHit = hit.EventUnitGo.GetComponent<NetworkIdentity>().netId;
            // Runs similar method on clients
            RpcSetupHealthBar(playerThatHit, netIdOfNewHit, hit.EventUnitGo);

            // if the person attacking is the same person on the server
            if (playerThatHit == gameObject.GetComponent<NetworkIdentity>().netId)
            {
                // If the first health bar hasn't been setup yet or has been reset
                if (netIdOfLastHit == 0)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }
                    enemyHealthBar.Setup(hit.EventUnitGo);
                    netIdOfLastHit = netIdOfNewHit;
                    return;
                }
                // If the hit enemy is the same as the previously hit one
                if (netIdOfLastHit == netIdOfNewHit)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }
                    enemyHealthBar.SetHealth();
                }
                else
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }
                    enemyHealthBar.Setup(hit.EventUnitGo);
                    netIdOfLastHit = netIdOfNewHit;
                }
            }
            else // if it wasn't the local player hitting it
            {
                if (netIdOfNewHit == netIdOfLastHit)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }
                    enemyHealthBar.SetHealth();
                }
            }
        }

        [ClientRpc]
        void RpcSetupHealthBar(uint playerId, uint enemyId, GameObject go)
        {
            if (!isClientOnly)
            {
                return;
            }

            playerThatHit = playerId;
            netIdOfNewHit = enemyId;
            // if the person attacking is the same person on the server
            if (playerThatHit == gameObject.GetComponent<NetworkIdentity>().netId)
            {
                healthBarInHierarchy.SetActive(true);
                // If the first health bar hasn't been setup yet or has been reset
                if (netIdOfLastHit == 0)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }

                    enemyHealthBar.Setup(go);
                    netIdOfLastHit = netIdOfNewHit;
                    return;
                }

                // If the hit enemy is the same as the previously hit one
                if (netIdOfLastHit == netIdOfNewHit)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }

                    enemyHealthBar.SetHealth();
                }
                else
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }

                    enemyHealthBar.Setup(go);
                    netIdOfLastHit = netIdOfNewHit;
                }
            }
            else // if it wasn't the local player hitting it
            {
                if (netIdOfNewHit == netIdOfLastHit)
                {
                    if (healthBarInHierarchy.active == false)
                    {
                        healthBarInHierarchy.SetActive(true);
                    }

                    enemyHealthBar.SetHealth();
                }
            }
        }
    }
}
