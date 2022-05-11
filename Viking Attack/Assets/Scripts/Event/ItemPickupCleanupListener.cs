using System;
using System.Collections;
using UnityEngine;

namespace Event
{
    public class ItemPickupCleanupListener : MonoBehaviour
    {
        
        private Guid pickUpGuid;

        [SerializeField] private float destroyTimer;
        
        private void Start()
        {
            
            EventSystem.Current.RegisterListener<PlayerItemPickupEventInfo>(OnHostPickup, ref pickUpGuid);
            
            
        }

        private void OnHostPickup(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            StartCoroutine(DestroyAfterTime(playerItemPickupEventInfo));
        }

        private IEnumerator DestroyAfterTime(PlayerItemPickupEventInfo playerItemPickupEventInfo)
        {
            yield return new WaitForSeconds(destroyTimer);
            
            if (playerItemPickupEventInfo.itemToDestroy)
            {
                Destroy(playerItemPickupEventInfo.itemToDestroy);
            }
        }
    }
}