using System;
using ItemNamespace;
using UnityEngine;


public class SceneSwitch : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Triggertooltip triggerTooltip;

    [SerializeField] private GameObject[] players;

    // Will start the teleportation sequence if a player enters the portal
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        Debug.Log("On trigger");
        if (triggerTooltip.GetKeyStatus())
        {
            players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var player in players)
            {
                if (player.GetComponent<PlayerTeleport>() != null)
                {
                    var position = gameObject.transform.position;
                    player.GetComponent<KillPlayer>().ChangeRespawnPoint(new Vector3(position.x, position.y, position.z - 20), true);
                    player.GetComponent<PlayerTeleport>().StartTeleport();
                }
            }
        }
    }

    public Triggertooltip GetTooltip()
    {
        return triggerTooltip;
    }
}