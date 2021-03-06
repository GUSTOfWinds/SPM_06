using UnityEngine;
using ItemNamespace;


namespace Event
{
    /**
     * @author Martin Kings
     */
    public abstract class EventInfo
    {
        public GameObject EventUnitGo;
        public string EventDescription;
    }


    // Die event class
    public class UnitDeathEventInfo : EventInfo
    {
        public float RespawnTimer;
        public ItemBase itemBase;
    }

    public class DebugEventInfo : EventInfo
    {
    }

    public class PlayerDamageEventInfo : EventInfo
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
        public uint netID;
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
        public uint playerNetId;
        public Vector3 hitPoint;
    }

    public class PlayerFatigueEventInfo : EventInfo
    {
    }

    public class ItemDropEventInfo : EventInfo
    {
        public ItemBase itemBase;
    }

    public class PlayerConnectEventInfo : EventInfo
    {
    }

    public class PlayerDeathEventInfo : EventInfo
    {
        public uint playerNetId;
    }

    public class EnemyRetreatingEventInfo : EventInfo
    {
        public uint netid;
    }
    
    public class BreakableDestroyedEventInfo : EventInfo
    {
        public float RespawnTimer;
    }
    public class BreakableRespawnEventInfo : EventInfo
    {
        // The spawner parent, makes sure that the enemy respawns at the same place 
        // it was placed in teh beginning of the game
        public Transform respawnParent;
    }
}