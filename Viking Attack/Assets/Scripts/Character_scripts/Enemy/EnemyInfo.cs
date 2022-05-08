using UnityEngine;
using ItemNamespace;

// WHO TO BLAME: Martin Kings

namespace ItemNamespace
{
    public class EnemyInfo : MonoBehaviour
    {
        [SerializeField] private float range;  // The range of the enemy attacks
        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks
        [SerializeField] private int damage; // the damage of the enemy attacks
        [SerializeField] private float cooldown; // float that will be reset to 0 after hitting the attackCooldown variable
        [SerializeField] private CharacterBase characterBase; // the scriptable object that we fetch all the variables from
        [SerializeField] private float chasingSpeedMultiplier; // the multiplier for the movement speed of the enemy (1 if to move at same pace as the regular movement speed)
        [SerializeField] private int moveSpeed; // movement speed of the enemy
        [SerializeField] private float health;
        [SerializeField] public float maxHealth;
        [SerializeField] private float experienceRadius;
        [Header("Insert the level you want the enemy to be")]
        [SerializeField] private int level;
        [SerializeField] private float experience;
        [SerializeField] private new string name;
        private bool hasHealthBarShown;
        private Transform respawnParent;
        private ItemBase drop;
        private SceneSwitch sceneSwitch;

        private void Awake()
        {
            // Updates the variables using the scriptable object
            experience = characterBase.GetExperience();
            name = characterBase.GetName();
            experienceRadius = characterBase.GetExperienceRadius();
            range = characterBase.GetRange();
            attackCooldown = characterBase.GetAttackCooldown();
            damage = characterBase.GetDamage();
            chasingSpeedMultiplier = characterBase.GetChasingSpeed();
            moveSpeed = characterBase.GetMovementSpeed();
            health = characterBase.GetMaxHealth();
            maxHealth = characterBase.GetMaxHealth();
            drop = characterBase.GetDrop();
            sceneSwitch = GameObject.FindGameObjectWithTag("Portal").GetComponent<SceneSwitch>();

        }

        public void Kill()
        {
            
                
            //TODO ADD EVENT LISTENER HERE, NEEDS TO FIND ALL LISTENERS FOR ENEMY DEATHS
            gameObject.SetActive(false);
            
        }

        public bool CheckHealthBarStatus()
        {
            return hasHealthBarShown;
        }

        public void SetRespawnAnchor(Transform p)
        {
            respawnParent = p;
        }

        public Transform GetRespawnParent()
        {
            return respawnParent;
        }

        public void SetHealthBarStatus(bool b)
        {
            hasHealthBarShown = b;
        }
        public ItemBase GetDrop()
        {
            return drop;
        }
        
        public void UpdateHealth(float difference)
        {
            health += difference;
            gameObject.transform.Find("Parent").gameObject.transform.Find("Health_bar").gameObject.GetComponent<EnemyHealthBar>().SetHealth();
            if (health <= 0)
            {
                sceneSwitch.DeadBoss(name);
                gameObject.GetComponent<EnemyInfo>().Kill();
            }
        }
    }
}