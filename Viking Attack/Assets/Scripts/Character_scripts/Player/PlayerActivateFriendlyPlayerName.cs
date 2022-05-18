using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    public class PlayerActivateFriendlyPlayerName : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */

        // The layermask of the other player
        [SerializeField] private LayerMask layerMask;

        // the prefab of the friendly name text to create.
        [SerializeField] private GameObject friendlyNamePrefab;

        // the hits detected by the spherecast
        private RaycastHit[] hits;

        // The netIDs of all players spotted in each frame
        private List<uint> instancesOfFriendliesSpotted;

        private List<uint> idsSpottedThisFrame;

        private List<GameObject> instancesOfFriendlyNames;

        private GameObject newFriendlyName;

        private Camera mainCamera;

        private uint tempNetId;


        private void Awake()
        {
            idsSpottedThisFrame = new List<uint>();
            instancesOfFriendliesSpotted = new List<uint>();
            instancesOfFriendlyNames = new List<GameObject>();
            mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            // All friendly players detected by the SphereCast
            hits = Physics.SphereCastAll(mainCamera.transform.position, 3,
                mainCamera.transform.forward, 30, layerMask);


            foreach (var hit in hits)
            {
                tempNetId = hit.transform.gameObject.GetComponent<NetworkIdentity>()
                    .netId;

                idsSpottedThisFrame.Add(tempNetId);
                // if the player wasn't spotted in the previous frame, will simply update previousHits and move to the next frame
                if (instancesOfFriendliesSpotted.Contains(tempNetId) == false)
                {
                    newFriendlyName = SetupFriendlyName(hit);
                    instancesOfFriendlyNames.Add(newFriendlyName);

                    instancesOfFriendliesSpotted.Add(tempNetId);
                }
            }

            for (int i = 0; i < instancesOfFriendlyNames.Count; i++)
            {
                tempNetId = instancesOfFriendlyNames[i].GetComponent<FriendlyNameDisplay>()
                    .GetPersonalNetID();
                // Checks if the enemy IDS that were spotted this frame exist in all instancesOfenemyHealthbars.
                if (idsSpottedThisFrame.Contains(tempNetId) == false)
                {
                    instancesOfFriendliesSpotted.Remove(tempNetId);
                    Destroy(instancesOfFriendlyNames[i]);
                    instancesOfFriendlyNames.Remove(instancesOfFriendlyNames[i]);
                }
            }

            idsSpottedThisFrame.Clear();
        }

        // Sets up the health bar instance and assigns proper values, must be cleaned up
        GameObject SetupFriendlyName(RaycastHit hit)
        {
            var go = Instantiate(friendlyNamePrefab,
                gameObject.transform); // creates the friendly name text instance
            uint id = hit.collider.gameObject.GetComponent<NetworkIdentity>().netId;

            go.GetComponent<FriendlyNameDisplay>().Setup(gameObject.transform, id, hit.collider.gameObject, mainCamera);
            return go;
        }
    }
}