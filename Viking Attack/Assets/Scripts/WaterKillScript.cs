using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterKillScript : MonoBehaviour
{
    void Update()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, 0.1f, LayerMask.GetMask("Water"))) 
        {
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    
    }
}
