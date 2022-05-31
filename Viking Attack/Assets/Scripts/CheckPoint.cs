using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
 
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player is here");
        if (other.CompareTag("Player"))
        {
            //change the respawn position for all player
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                p.GetComponent<KillPlayer>().ChangeRespawnPoint(gameObject.transform.position, false);
            }
            //First destory collider, the player may interact with runestone now 
            Destroy(gameObject.GetComponent<BoxCollider>());
            Invoke("DisableVFX", 2f);
        }
    }

    private void DisableVFX()
    {
        Destroy(gameObject);
    }

}
