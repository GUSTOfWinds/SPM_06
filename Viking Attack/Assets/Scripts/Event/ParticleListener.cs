using System;
using UnityEngine;

namespace Event
{
    public class ParticleListener : MonoBehaviour
    {
        private Guid ParticleEventGuid;
        [SerializeField] private GameObject[] particles;
        private void Start()
        {
            EventSystem.Current.RegisterListener<EnemyHitEvent>(OnUnitHit, ref ParticleEventGuid);
            
        }
        
        private void Update()
        {
            
        }
        private void OnUnitHit(EnemyHitEvent eventInfo)
        {
            if(particles[0] != null)
            {
                GameObject temp = Instantiate(particles[0],eventInfo.hitPoint, Quaternion.Euler(0,0,0));
                Destroy(temp,1);
            }
        }
        
    }
}