using UnityEngine;

public class KillPlayer : MonoBehaviour
{
   

    public void PlayerRespawn()
    {
        transform.position = new Vector3(0f, 10f, 0f);
        gameObject.GetComponent<GlobalPlayerInfo>().SetHealth(gameObject.GetComponent<GlobalPlayerInfo>().GetMaxHealth());
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }
}
