using System;
using Event;
using ItemNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class SceneSwitch : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] public bool keyIsFound;

    private Triggertooltip triggerTooltip;
    
    private Guid portalEventGuid;


    private void Start()
    {
        triggerTooltip = GetComponentInChildren<Triggertooltip>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        if (triggerTooltip.GetKeyStatus())
        {
            Debug.Log("Key has been picked up. Will portal player now");
            
            // foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            // {
            //     DontDestroyOnLoad(player);
            // }

            // Add teleports the player here
            //NetworkManager.singleton.ServerChangeScene("TerrainIsland2");
        }
    }

}