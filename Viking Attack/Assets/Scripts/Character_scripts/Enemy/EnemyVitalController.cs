using System;
using DefaultNamespace;
using Event;
using Mirror;
using UnityEngine;

public class EnemyVitalController : NetworkBehaviour
{
    float maxHealth;
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] public float waitTime;
    [SerializeField]private bool hasDied;
    
    [SerializeField][SyncVar(hook = nameof(OnHealthChangedHook))]float currentHealth = 100f;
    //TODO Vi behöver lägga till så nuvarande HP uppdateras. EnemyHealthBarController ska uppdatera alla värden där.
    //spara maxvärdet så vi kan räkna ut procent 
    void Start()
    {
        currentHealth = characterBase.GetMaxHealth();
        maxHealth = currentHealth;
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
            RespawnTimer = waitTime
        };
            
            
        EventSystem.Current.FireEvent(unitDeathEventInfo);
    }
    

    //körs på alla klienter efter att syncvarens värde ändras
    void OnHealthChangedHook(float old, float @new)
    {
        //invoka vårt event, passa med vårt nya värde (syncvaren är uppdaterad när hooken körs)
        this.OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateHealth(float change) => UpdateHealth(change);

    private void UpdateHealth(float change)
    {
        
        if (base.isServer)
        {
            
            //clampa värdet så vi inte kan få mer hp än maxvärdet
            currentHealth = Mathf.Clamp(currentHealth += change, -Mathf.Infinity, maxHealth);

            if(currentHealth <= 0f)
            {
                this.OnDeath?.Invoke(this);
                Die();
            }
        }
        else
            CmdUpdateHealth(change);
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    //andra script kan registrera på detta event
    public event Action<float> OnHealthChanged;

    //OBS KÖRS ENDAST PÅ SERVERN
    public event Action<EnemyVitalController> OnDeath;
}