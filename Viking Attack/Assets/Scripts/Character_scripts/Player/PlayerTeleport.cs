using UnityEngine;

namespace ItemNamespace
{
    public class PlayerTeleport : MonoBehaviour
    {
        [SerializeField] private GameObject teleportSpot;
        private Vector3 portPosition;

        private void Start()
        {
            portPosition = teleportSpot.transform.position;
        }

        public void StartTeleport()
        {
            
        }
        
    }
}