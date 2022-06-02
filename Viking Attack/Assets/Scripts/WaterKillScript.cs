using System;
using Event;
using Mirror;
using UnityEngine;

public class WaterKillScript : MonoBehaviour
{
    /*
        @Author Love Strignert - lost9373
    */
    //Checks if the player hits the water if true respawn player

    void Update()
    {
        if (Physics.Raycast(
                new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f,
                    transform.transform.position.y), Vector3.down, 0.1f,
                LayerMask.GetMask("Water")))
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