using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ItemNamespace;
using Event;
using Mirror;
using UnityEngine;

public class BreakableBehavior : MonoBehaviour
{
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] public float waitTime;
    [SerializeField] private bool hasDied;
    private Collider[] sphereColliders;
    [SerializeField] private LayerMask layerMask;


    [SerializeField] [SyncVar] private float maxHealth;

    
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] materials;
    [SerializeField] private Material hitMaterial;
    // Start is called before the first frame update
    public void Break()
    {
        sphereColliders =
                    Physics.OverlapSphere(transform.position, characterBase.GetExperienceRadius(), layerMask);
        foreach (var coll in sphereColliders)
        {
            // Updates both the client and the player
            RpcIncreaseExperience(coll.gameObject, 20f);
            coll.transform.GetComponent<GlobalPlayerInfo>().IncreaseExperience(20f);
        }

        this.OnDeath?.Invoke(this);
        Die();
    }
    private void RpcIncreaseExperience(GameObject player, float exp)
    {
        player.GetComponent<GlobalPlayerInfo>().IncreaseExperience(exp);
    }


    public void Die()
    {
        if (hasDied)
            return;
        hasDied = true;
        EventInfo unitDeathEventInfo = new UnitDeathEventInfo
        {
            EventUnitGo = gameObject,
            EventDescription = "Unit " + gameObject.name + " has died.",
            RespawnTimer = waitTime,
        };
        EventSystem.Current.FireEvent(unitDeathEventInfo);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public event Action<BreakableBehavior> OnDeath;
}
