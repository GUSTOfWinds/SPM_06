using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ItemNamespace;
using Event;
using Mirror;
using UnityEngine;

public class BreakableBehavior : NetworkBehaviour
{
    [SerializeField] public float waitTime;
    [SerializeField] private Renderer render;
    [SerializeField] private bool hasDied;
    private Collider[] sphereColliders;
    [SerializeField] private LayerMask layerMask;
    public GameObject fractured;

    // Start is called before the first frame update

    public void Break()
    {
        
        sphereColliders =
                    Physics.OverlapSphere(transform.position, 20f, layerMask);
        foreach (var coll in sphereColliders)
        {
            
            // Updates both the client and the player
            RpcIncreaseExperience(coll.gameObject, 7f);
            coll.transform.GetComponent<GlobalPlayerInfo>().IncreaseExperience(7f);
        }

        this.OnDeath?.Invoke(this);
        Die();
    }
    [ClientRpc]
    private void RpcIncreaseExperience(GameObject player, float exp)
    {
        player.GetComponent<GlobalPlayerInfo>().IncreaseExperience(7f);
        


    }
    [Command(requiresAuthority = false)]
    public void CmdBreak() => Break();
    private void Start()
    {
        //render.enabled = true;
    }


    public void Die()
    {
        Debug.Log("dieBehavior");
        if (hasDied)
            return;
        hasDied = true;
        EventInfo breakableDestroyedEventInfo = new BreakableDestroyedEventInfo
        {
            EventUnitGo = gameObject,
            EventDescription = "Unit " + gameObject.name + " has died.",
            RespawnTimer = waitTime,
        };
        GameObject fractur = Instantiate(fractured, transform.position, Quaternion.identity);
        NetworkServer.Spawn(fractur);
        EventSystem.Current.FireEvent(breakableDestroyedEventInfo);
    }

    
    public event Action<BreakableBehavior> OnDeath;
}
