using Mirror;
using UnityEngine;


public class KillPlayer : NetworkBehaviour
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

    //Change player respawn position when reach checkpoint  By Jiang // Martin Kings
    public void ChangeRespawnPoint(Vector3 checkPoint, bool bothPlayers)
    {
        // if both players should have their respawn updated
        if (bothPlayers)
        {
            if (isServer)
            {
                RpcChangeRespawnPoint(checkPoint);
            }
            else
            {
                CmdChangeRespawnPoint(checkPoint);
            }
        }

        respawnPoint = checkPoint;
    }

    /*
     * @author Martin Kings
     */
    [ClientRpc]
    private void RpcChangeRespawnPoint(Vector3 newRespawnPos)
    {
        respawnPoint = newRespawnPos;
    }

    /*
    * @author Martin Kings
    */
    [Command(requiresAuthority = false)]
    private void CmdChangeRespawnPoint(Vector3 newRespawnPos)
    {
        respawnPoint = newRespawnPos;
    }
}