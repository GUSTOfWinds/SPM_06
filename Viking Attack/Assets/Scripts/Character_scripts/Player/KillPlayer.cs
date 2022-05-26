using UnityEngine;


public class KillPlayer : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    private Vector3 respawnPoint;
    private void Start()
    {
        respawnPoint = GameObject.FindGameObjectWithTag("PlayerRespawnAnchor").transform.position;
    }

    public void PlayerRespawn()
    {
        transform.position = respawnPoint;
        Invoke("HealthBack", 3f);
    }

    private void HealthBack()
    {
        gameObject.GetComponent<GlobalPlayerInfo>()
            .SetHealth(gameObject.GetComponent<GlobalPlayerInfo>().GetMaxHealth());
        gameObject.GetComponent<GlobalPlayerInfo>().UpdateHealth(0);
    }
    //Change player respawn position when reach checkpoint  By Jiang
    public void changeRespawnPoint(Vector3 checkPoint)
    {
        respawnPoint = checkPoint;
    }
}