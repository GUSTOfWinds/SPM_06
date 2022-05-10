using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Security;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    // WHO TO BLAME: Martin Kings
    public class PlayerActivateEnemyHealthBar : NetworkBehaviour
    {
        // The layermask of the enemies
        [SerializeField] private LayerMask layerMask;

        // the prefab of the enemy health bar to create.
        [SerializeField] private GameObject enemyHealthPrefab;

        // the hits detected by the spherecast
        private RaycastHit[] hits;

        // The netIDs of all enemies spotted in each frame
        private List<uint> instancesOfEnemiesSpotted;

        private List<uint> idsSpottedThisFrame;

        // All Enemy health bars that exist and belong to an enemy
        private List<GameObject> instancesOfEnemyHealthBars;
        

        private Camera mainCamera;


        private void Awake()
        {
            idsSpottedThisFrame = new List<uint>();
            instancesOfEnemiesSpotted = new List<uint>();
            instancesOfEnemyHealthBars = new List<GameObject>();
            mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        }

        void FixedUpdate()
        {

            if (!isLocalPlayer) return;
            // All enemies detected by the SphereCast
            hits = Physics.SphereCastAll(mainCamera.transform.position, 3,
                mainCamera.transform.forward, 10, layerMask);

            // Instantiates an health bar for each enemy in sight if one is missing
            foreach (var hit in hits)
            {
                idsSpottedThisFrame.Add(hit.transform.GetComponent<NetworkIdentity>().netId);
                // if the enemy wasn't spotted in the previous frame, will simply update previousHits and move to the next frame
                if (instancesOfEnemiesSpotted.Contains(hit.transform.gameObject.GetComponent<NetworkIdentity>()
                        .netId) == false)
                {
                    GameObject go = SetupHealthBar(hit);
                    instancesOfEnemyHealthBars.Add(go);
                    
                    // Adds to all enemy instances (saves the instanceID)
                    instancesOfEnemiesSpotted.Add(hit.transform.gameObject.GetComponent<NetworkIdentity>().netId);
                }
            }
    
            // Loops through and destroys the healthbars that aren't visible in this frame
            for (int i = 0; i < instancesOfEnemyHealthBars.Count; i++)
            {
                // Checks if the enemy IDS that were spotted this frame exist in all instancesOfenemyHealthbars.
                if (idsSpottedThisFrame.Contains(instancesOfEnemyHealthBars[i].GetComponent<EnemyHealthBar>().GetPersonalNetID()) == false)
                {
                    instancesOfEnemiesSpotted.Remove(instancesOfEnemyHealthBars[i].GetComponent<EnemyHealthBar>().GetPersonalNetID());
                    Destroy(instancesOfEnemyHealthBars[i]);
                    instancesOfEnemyHealthBars.Remove(instancesOfEnemyHealthBars[i]);
                }
            }
            idsSpottedThisFrame.Clear();
        }


        // Sets up the health bar instance and assigns proper values, must be cleaned up
        GameObject SetupHealthBar(RaycastHit hit)
        {
            var enemy = hit.collider.transform;
            var uiTargetToFollow = enemy.Find("Overhead").gameObject.transform; // sets the target location
            var go = Instantiate(enemyHealthPrefab,
                gameObject.transform); // creates the health bar instance

            go.GetComponent<EnemyHealthBar>().Setup(gameObject.transform, uiTargetToFollow, enemy,
                enemy.GetComponent<EnemyInfo>(), mainCamera
                , enemy.GetComponent<NetworkIdentity>().netId);

            return go;
        }
    }
}