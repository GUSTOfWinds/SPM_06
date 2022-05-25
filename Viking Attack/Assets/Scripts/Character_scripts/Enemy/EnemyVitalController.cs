using System;
using System.Collections;
using System.Collections.Generic;
using ItemNamespace;
using Event;
using Mirror;
using UnityEngine;


public class EnemyVitalController : NetworkBehaviour
{
    /**
     * @author Martin, Love, Victor
     */
    public Dictionary<uint,float> aggroCounter;
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] public float waitTime;
    [SerializeField] private bool hasDied;
    private Collider[] sphereColliders;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] [SyncVar(hook = nameof(OnHealthChangedHook))]
    float currentHealth;

    [SerializeField] [SyncVar] private float maxHealth;

    private EnemyInfo enemyInfo;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] materials;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;


    //spara maxvärdet så vi kan räkna ut procent 
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        currentHealth = characterBase.GetMaxHealth();
        maxHealth = currentHealth;
        enemyInfo = gameObject.GetComponent<EnemyInfo>();
        enemyInfo.PlayerScale();

        materials = new Material[skinnedMeshRenderer.materials.Length + 1];
        Array.Copy(skinnedMeshRenderer.materials, materials, skinnedMeshRenderer.materials.Length);
        materials[materials.Length - 1] = hitMaterial;

        aggroCounter = new Dictionary<uint, float>();
    }

    private void OnConnectedToServer()
    {
        UpdateHealth(0);
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

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    //körs på alla klienter efter att syncvarens värde ändras
    void OnHealthChangedHook(float old, float @new)
    {
        //invoka vårt event, passa med vårt nya värde (syncvaren är uppdaterad när hooken körs)
        this.OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateHealth(float change, uint player) => UpdateHealth(change, player);

    [Command(requiresAuthority = false)]
    public void CmdUpdateHealth(float change) => UpdateHealth(change);

    // Updates the health of the enemy when being damaged or healed.
    // When dying, it will add experience to all players in proximity
    public void UpdateHealth(float change, uint player)
    {
        if (base.isServer)
        {
            if (change != 0)
            {
                gameObject.GetComponent<EnemyInfo>().PlayerScale();
            }

            //clampa värdet så vi inte kan få mer hp än maxvärdet
            currentHealth = Mathf.Clamp(currentHealth += change, -Mathf.Infinity, maxHealth);
            if (change < 0)
            {
                if(aggroCounter.ContainsKey(player))
                {
                    aggroCounter[player] += change;
                }else
                {
                    aggroCounter.Add(player,change);
                }

                RpcBlinkOnHit(); // Plays for client
                StartCoroutine(BlinkOnHit());

                RpcPlayHitSound(); // Plays for client
                PlayHitSound();
                EventInfo enemyTakesDamage = new EnemyHitEvent
                {
                    EventUnitGo = gameObject,
                    playerNetId = player
                };
                EventSystem.Current.FireEvent(enemyTakesDamage);
            }

            if (currentHealth <= 0f)
            {
                if (gameObject.GetComponent<EnemyAttack>() != null)
                {
                    gameObject.GetComponent<EnemyAttack>().StopCoroutine("FinishAttack");
                }

                sphereColliders =
                    Physics.OverlapSphere(transform.position, characterBase.GetExperienceRadius(), layerMask);
                foreach (var coll in sphereColliders)
                {
                    // Updates both the client and the player
                    RpcIncreaseExperience(coll.gameObject, enemyInfo.GetExperience());
                    coll.transform.GetComponent<GlobalPlayerInfo>().IncreaseExperience(enemyInfo.GetExperience());
                }

                if (gameObject.GetComponent<EnemyAIScript>() != null)
                    gameObject.GetComponent<EnemyAIScript>().RpcBeforeDying(
                        gameObject.GetComponent<EnemyAIScript>().GetSpawnPoint,
                        gameObject.GetComponent<EnemyAIScript>().GetRoamingPoint);
                this.OnDeath?.Invoke(this);
                Die();
            }
        }
        else
            CmdUpdateHealth(change);
    }

    public void UpdateHealth(float change)
    {
        if (base.isServer)
        {
            if (change != 0)
            {
                gameObject.GetComponent<EnemyInfo>().PlayerScale();
            }

            //clampa värdet så vi inte kan få mer hp än maxvärdet
            currentHealth = Mathf.Clamp(currentHealth += change, -Mathf.Infinity, maxHealth);

            if (currentHealth <= 0f)
            {
                StartCoroutine(BlinkOnHit());
                if (gameObject.GetComponent<EnemyAttack>() != null)
                {
                    gameObject.GetComponent<EnemyAttack>().StopCoroutine("FinishAttack");
                }

                sphereColliders =
                    Physics.OverlapSphere(transform.position, characterBase.GetExperienceRadius(), layerMask);
                foreach (var coll in sphereColliders)
                {
                    // Updates both the client and the player
                    RpcIncreaseExperience(coll.gameObject, enemyInfo.GetExperience());
                    coll.transform.GetComponent<GlobalPlayerInfo>().IncreaseExperience(enemyInfo.GetExperience());
                }

                if (gameObject.GetComponent<EnemyAIScript>() != null)
                    gameObject.GetComponent<EnemyAIScript>().RpcBeforeDying(
                        gameObject.GetComponent<EnemyAIScript>().GetSpawnPoint,
                        gameObject.GetComponent<EnemyAIScript>().GetRoamingPoint);
                this.OnDeath?.Invoke(this);
                Die();
            }
        }
        else
            CmdUpdateHealth(change);
    }

    // Plays the hit color on client
    [ClientRpc]
    private void RpcBlinkOnHit()
    {
        if (isServer)
        {
            return;
        }

        StartCoroutine(BlinkOnHit());
    }
    
    private IEnumerator BlinkOnHit()
    {
        Material[] temp = skinnedMeshRenderer.materials;
        skinnedMeshRenderer.materials = materials;
        yield return new WaitForSeconds(0.1f);
        skinnedMeshRenderer.materials = temp;
    }
    
    [ClientRpc]
    private void RpcPlayHitSound()
    {
        if (isServer)
        {
            return;
        }

        PlayHitSound();
    }

    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            //audioSource.Stop();
            audioSource.PlayOneShot(hitSound);
        }
    }
    
    // Ships experience to clients, makes experience within proximity possible
    [ClientRpc]
    private void RpcIncreaseExperience(GameObject player, float exp)
    {
        player.GetComponent<GlobalPlayerInfo>().IncreaseExperience(exp);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void PlayerScaleHealthUpdate(float hp, float maxhp)
    {
        maxHealth = maxhp;
        currentHealth = hp;
        if (currentHealth > 0)
        {
            UpdateHealth(0);
        }
    }

    //Other scripts that can change this event
    public event Action<float> OnHealthChanged;

    //OBS Is only run on the server
    public event Action<EnemyVitalController> OnDeath;
}