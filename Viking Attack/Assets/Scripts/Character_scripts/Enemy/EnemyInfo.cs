using System;
using Mirror;
using UnityEngine;

namespace ItemNamespace
{
    public class EnemyInfo : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private float range; // The range of the enemy attacks

        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks

        [SerializeField] private float damage; // the damage of the enemy attacks

        // float that will be reset to 0 after hitting the attackCooldown variable
        [SerializeField] private float cooldown;

        [SerializeField]
        private CharacterBase characterBase; // the scriptable object that we fetch all the variables from

        [SerializeField] private float
            chasingSpeedMultiplier; // the multiplier for the movement speed of the enemy (1 if to move at same pace as the regular movement speed)

        [SerializeField] private float moveSpeed; // movement speed of the enemy
        [SerializeField] private float health;
        [SerializeField] public float maxHealth;
        private float experienceRadius;

        [Header("Insert the level you want the enemy to be")] [SerializeField]
        private int level;

        private float experience;
        [SerializeField] private new string name;
        private bool hasHealthBarShown;
        private Transform respawnParent;
        [SerializeField] private GameObject drop; // insert item to be dropped from prefab

        [SerializeField] private int dropChance; // insert value for dropchance
        private GameObject[] players;
        private int scale;

        private void Awake()
        {
            scale = 1;
            // Updates the variables using the scriptable object
            experience = characterBase.GetExperience() * level;
            experienceRadius = characterBase.GetExperienceRadius();
            range = characterBase.GetRange();
            attackCooldown = characterBase.GetAttackCooldown();
            damage = characterBase.GetDamage();
            chasingSpeedMultiplier = characterBase.GetChasingSpeed();
            moveSpeed = characterBase.GetMovementSpeed();
            maxHealth = characterBase.GetMaxHealth();
            health = characterBase.GetMaxHealth();
            
            if (level > 1)
            {
                for (int i = 1; i < level; i++)
                {
                    damage *= 1.3f;
                    maxHealth *= 1.3f;
                    health = maxHealth;
                }
            }
        }

        public int GetEnemyLevel()
        {
            return level;
        }

        public void SetRespawnAnchor(Transform p)
        {
            respawnParent = p;
        }

        public Transform GetRespawnParent()
        {
            return respawnParent;
        }

        public GameObject GetDrop()
        {
            return drop;
        }

        public int GetDropChance()
        {
            return dropChance;
        }

        public string GetName()
        {
            return name;
        }

        public float GetDamage()
        {
            return damage;
        }

        public CharacterBase GetCharacterBase()
        {
            return characterBase;
        }

        public float GetExperience()
        {
            return experience;
        }

        // increases damage and health if there are multiple players
        public void PlayerScale()
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 1 &&
                scale != players.Length) // checks the scale to make sure the enemy doesn't get scaled 
                // several times
            {
                scale = players.Length;

                maxHealth *= (float) Math.Pow(1.3, players.Length * 1.45);
                damage *= (float) Math.Pow(1.3, players.Length * 1.33);
                health = maxHealth;
                
            }
            gameObject.GetComponent<EnemyVitalController>().PlayerScaleHealthUpdate(health, maxHealth);
        }

        public void SetDropItem(GameObject drop) => this.drop = drop;
        public void SetDropChance(int chance) => dropChance = chance;
    }
}