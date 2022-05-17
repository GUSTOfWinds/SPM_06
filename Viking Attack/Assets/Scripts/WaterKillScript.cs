using UnityEngine;

public class WaterKillScript : MonoBehaviour
{
    //Checks if the player hits the water if true respawn player
    void Update()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, 0.1f, LayerMask.GetMask("Water"))) 
        {
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    
    }
}