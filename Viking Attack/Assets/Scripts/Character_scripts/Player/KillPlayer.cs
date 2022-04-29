using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
   

    public void PlayerRespawn()
    {
        transform.position = new Vector3(0f, 10f, 0f);
        gameObject.GetComponent<GlobalPlayerInfo>().health = gameObject.GetComponent<GlobalPlayerInfo>().maxHealth;
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }
}
