using System;
using UnityEngine;

namespace Event
{
    public class DebugListener : MonoBehaviour
    {
        private Guid DebugEventGuid;
        private void Start()
        {
            EventSystem.Current.RegisterListener<DebugEventInfo>(OnUnitDied, ref DebugEventGuid);
            
        }
        
        

        void OnUnitDied(DebugEventInfo debugEventInfo)
        {
            
            Debug.Log(debugEventInfo.EventDescription);
        }


    }
}