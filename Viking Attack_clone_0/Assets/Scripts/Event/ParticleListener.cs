using System;
using System.Collections;
using UnityEngine;

namespace Event
{
    public class ParticleListener : MonoBehaviour
    {
        private Guid ParticleEventGuid;
        private void Start()
        {
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnUnitDied, ref ParticleEventGuid);
            
        }
        
        private void Update()
        {
            // testing remove
            if (Input.GetKeyDown(KeyCode.V))
            {
                EventSystem.Current.UnregisterListener(ParticleEventGuid);
            }
        }
        
        

        void OnUnitDied(UnitDeathEventInfo unitDeathEventInfo)
        {
            StartCoroutine(PlayParticles(unitDeathEventInfo));
        }

        IEnumerator PlayParticles(UnitDeathEventInfo unitDeathEventInfo)
        {
            yield return new WaitForSeconds(unitDeathEventInfo.RespawnTimer - 0.5f);
            unitDeathEventInfo.EventUnitGo.GetComponent<ParticleSystem>().Play();
        }
    }
}