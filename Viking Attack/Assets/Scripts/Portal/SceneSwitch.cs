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

        if (triggerTooltip.GetKeyStatus())
        {
            players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var player in players)
            {
                if (player.GetComponent<PlayerTeleport>() != null)
                {
                    player.GetComponent<PlayerTeleport>().StartTeleport();
                }
            }
        }
    }
}