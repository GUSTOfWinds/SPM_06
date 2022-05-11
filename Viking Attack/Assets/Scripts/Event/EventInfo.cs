using ItemNamespace;
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
        public Transform respawnParent;
    }

    public class PlayerLevelUpEventInfo : EventInfo
    {
        
    }

    public class PlayerItemPickupEventInfo : EventInfo
    {
        public ItemBase itemBase;
        public GameObject itemToDestroy;
    }

    public class PlayerEatingEventInfo : EventInfo
    {
        
    }
    public class EnemyHitEvent : EventInfo
    {
        public GameObject enemy;
        public Vector3 hitPoint;
    }
    
    public class PlayerFatigueEventInfo : EventInfo
    {
        
    }
}