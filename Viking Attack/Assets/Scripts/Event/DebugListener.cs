using System;
using UnityEngine;

namespace Event
{
    /**
     * @author Martin Kings
     */
    public class DebugListener : MonoBehaviour
    {
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