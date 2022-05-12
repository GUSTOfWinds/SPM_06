using UnityEngine;

public class KillPlayer : MonoBehaviour
{
   

    public void PlayerRespawn()
    {
        transform.position = new Vector3(10f, -30f, 10f);
        Invoke("HealthBack", 3f);
    }

    private void HealthBack()
    {
        gameObject.GetComponent<GlobalPlayerInfo>().SetHealth(gameObject.GetComponent<GlobalPlayerInfo>().GetMaxHealth());
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }


}
