using System;
using System.Collections;
using System.Collections.Generic;
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

            if (isLocalPlayer)
            {
                StartCoroutine(DelayedEnemyScale());
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

        // Will pick up any death event on the server, if the killed enemy was the last hit enemy, the 
        // health bar will be reset.
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
        void RpcOnEnemyDeath(uint netIdOfDeadEnemy)
        {
            if (!isClientOnly)
            {
                return;
            }
            
            if (netIdOfDeadEnemy == netIdOfLastHit)
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
            
            
            RpcSetupHealthBar(playerThatHit, netIdOfNewHit, hit.EventUnitGo);
            
            // if the person attacking is the same person on the server
            if (playerThatHit == gameObject.GetComponent<NetworkIdentity>().netId)
            {
                // If the first health bar hasn't been setup yet
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
            else
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
                Debug.Log("nu kommer jag in på klient");
                healthBarInHierarchy.SetActive(true);
                // If the first health bar hasn't been setup yet
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
            else
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