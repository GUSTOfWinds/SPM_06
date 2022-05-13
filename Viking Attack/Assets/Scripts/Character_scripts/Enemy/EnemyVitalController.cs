using System;
using Character_scripts.Player;
using Event;
using Mirror;
using UnityEngine;

namespace Character_scripts.Enemy
{
    /**
     * @author Martin Kings
     */
    public class EnemyVitalController : NetworkBehaviour
    {
        float maxHealth;
        [SerializeField] private CharacterBase characterBase;
        [SerializeField] public float waitTime;
        [SerializeField] private bool hasDied;
        private Collider[] sphereColliders;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] [SyncVar(hook = nameof(OnHealthChangedHook))]
        float currentHealth = 100f;

        private EnemyInfo enemyInfo;

        //spara maxvärdet så vi kan räkna ut procent 
        void Start()
        {
            currentHealth = characterBase.GetMaxHealth();
            maxHealth = currentHealth;
            enemyInfo = gameObject.GetComponent<EnemyInfo>();
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


        //körs på alla klienter efter att syncvarens värde ändras
        void OnHealthChangedHook(float old, float @new)
        {
            //invoka vårt event, passa med vårt nya värde (syncvaren är uppdaterad när hooken körs)
            this.OnHealthChanged?.Invoke(currentHealth / maxHealth);
        }

        [Command(requiresAuthority = false)]
        public void CmdUpdateHealth(float change) => UpdateHealth(change);

        // Updates the health of the enemy when being damaged or healed.
        // When dying, it will add experience to all players in proximity
        public void UpdateHealth(float change)
        {
            if (base.isServer)
            {
                //clampa värdet så vi inte kan få mer hp än maxvärdet
                currentHealth = Mathf.Clamp(currentHealth += change, -Mathf.Infinity, maxHealth);
                if (currentHealth <= 0f)
                {
                    sphereColliders =
                        Physics.OverlapSphere(transform.position, characterBase.GetExperienceRadius(), layerMask);
                    foreach (var coll in sphereColliders)
                    {
                        // Updates both the client and the player
                        RpcIncreaseExperience(coll.gameObject, enemyInfo.GetExperience());
                        coll.transform.GetComponent<GlobalPlayerInfo>().IncreaseExperience(enemyInfo.GetExperience());
                    }

                    this.OnDeath?.Invoke(this);
                    Die();
                }
            }
            else
                CmdUpdateHealth(change);
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
        }

        //andra script kan registrera på detta event
        public event Action<float> OnHealthChanged;

        //OBS KÖRS ENDAST PÅ SERVERN
        public event Action<EnemyVitalController> OnDeath;
    }
}