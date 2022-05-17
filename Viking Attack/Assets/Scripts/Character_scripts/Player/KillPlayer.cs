using UnityEngine;


public class KillPlayer : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    public void PlayerRespawn()
    {
        transform.position = GameObject.FindGameObjectWithTag("PlayerRespawnAnchor").transform.position;
        Invoke("HealthBack", 3f);
    }

    private void HealthBack()
    {
        gameObject.GetComponent<GlobalPlayerInfo>()
            .SetHealth(gameObject.GetComponent<GlobalPlayerInfo>().GetMaxHealth());
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }
}