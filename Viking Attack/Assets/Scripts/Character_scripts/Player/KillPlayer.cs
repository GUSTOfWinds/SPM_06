using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
   

    public void PlayerRespawn()
    {
        transform.position = new Vector3(0f, 10f, 0f);
        //give health back 2s later
        Invoke("HealthBack", 2);
    }

    private void HealthBack()
    {
        gameObject.GetComponent<GlobalPlayerInfo>().SetHealth(gameObject.GetComponent<GlobalPlayerInfo>().GetMaxHealth());
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }
}
