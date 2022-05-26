using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnHit(GameObject other)
    {
    Debug.Log("Someone here");
        if(other.CompareTag("Player"))
        {
            //change the respawn position for all player
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject p in players)
            {
                p.GetComponent<KillPlayer>().changeRespawnPoint(gameObject.transform.position);
            }
            Destroy(gameObject);
        }
    }
}
