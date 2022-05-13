using System;
using UnityEngine;

namespace Event
{
    public class DebugListener : MonoBehaviour
    {
        /**
         * @author Martin Kings
         */
        private Guid debugEventGuid;
        private void Start()
        {
            EventSystem.Current.RegisterListener<DebugEventInfo>(OnUnitDied, ref debugEventGuid);
            
        }
        
        

        void OnUnitDied(DebugEventInfo debugEventInfo)
        {
            
            Debug.Log(debugEventInfo.EventDescription);
        }


    }
}