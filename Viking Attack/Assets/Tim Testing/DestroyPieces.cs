using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyPieces : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject fractured;
    public float targetTime = 3.0f;

    

    void timerEnded()
    {
        NetworkServer.Destroy(fractured);
    }

    // Update is called once per frame
    void Update()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            timerEnded();
        }
        
    }
}
