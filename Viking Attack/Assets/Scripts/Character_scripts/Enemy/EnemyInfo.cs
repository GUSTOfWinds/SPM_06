using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using ItemNamespace;
using Mirror;
using UnityEditor;


namespace ItemNamespace
{
    public class EnemyInfo : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private float range; // The range of the enemy attacks

        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks

        [SerializeField] private int damage; // the damage of the enemy attacks

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
        private EnemyVitalController enemyVitalController;

        private void Start()
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
            enemyVitalController = gameObject.GetComponent<EnemyVitalController>();
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

        public CharacterBase GetCharacterBase()
        {
            return characterBase;
        }

        public float GetExperience()
        {
            return experience;
        }

        //get health back when moving back to default status
        public void BackToDefault()
        {
            gameObject.GetComponent<EnemyInfo>().health = maxHealth;
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
                damage *= (int) Math.Pow(1.3, players.Length * 1.33);
                health = maxHealth;
                enemyVitalController.PlayerScaleHealthUpdate(health, maxHealth);
            }
        }
    }
}