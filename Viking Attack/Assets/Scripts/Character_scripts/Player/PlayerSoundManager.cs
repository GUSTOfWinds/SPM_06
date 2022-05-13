using Mirror;
using UnityEngine;

namespace Character_scripts.Player
{
    /**
     * @author Martin Kings
     */
    public class PlayerSoundManager : NetworkBehaviour
    {
        // The gameobject containing the listener for each player taking damage.
        [SerializeField] GameObject playerSoundContainer;

        // Sets the gameobject containing the listener for the local player object to active
        public override void OnStartLocalPlayer()
        {
            playerSoundContainer.SetActive(true);
        }
    }
}