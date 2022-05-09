using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    // Main class, contains a description
    public abstract class EventInfo
    {
        public GameObject EventUnitGo;
        public string EventDescription;
    }

    
    // Die event class
    public class UnitDeathEventInfo : EventInfo
    {
        public float RespawnTimer;
    }

    public class DebugEventInfo : EventInfo
    {
        
    }

    public class DamageEventInfo : EventInfo
    {
        public GameObject target;
    }

    public class EnemyRespawnEventInfo : EventInfo
    {
        // The spawner parent, makes sure that the enemy respawns at the same place 
        // it was placed in teh beginning of the game
        public Transform parent;
    }

    public class PlayerLevelUpEventInfo : EventInfo
    {
        
    }
}