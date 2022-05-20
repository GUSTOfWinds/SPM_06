using Event;
using Mirror;
using UnityEngine;
public class WaterKillScript : MonoBehaviour
{
    //Checks if the player hits the water if true respawn player
    void Update()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, 0.1f, LayerMask.GetMask("Water"))) 
        {
            EventInfo playerDeathEvent = new PlayerDeathEventInfo
            {
                EventUnitGo = gameObject,
                playerNetId = gameObject.GetComponent<NetworkIdentity>().netId
            };
            EventSystem.Current.FireEvent(playerDeathEvent);
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    
    }
}