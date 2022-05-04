using System;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    public class PlayerSoundManager : NetworkBehaviour
    {
        // The gameobject containing the listener for each player taking damage.
        [SerializeField] GameObject playerDamageController;

        // Sets the gameobject containing the listener for the local player object to active
        public override void OnStartLocalPlayer()
        {
            playerDamageController.SetActive(true);
        }
    }
}