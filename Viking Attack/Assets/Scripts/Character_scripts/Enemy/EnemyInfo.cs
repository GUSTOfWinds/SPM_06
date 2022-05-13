using System;
using System.Collections;
using UnityEngine;
using ItemNamespace;
using UnityEditor;

// WHO TO BLAME: Martin Kings

namespace ItemNamespace
{
    public class EnemyInfo : MonoBehaviour
    {
        [SerializeField] private float range; // The range of the enemy attacks
        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks

        [SerializeField] private int damage; // the damage of the enemy attacks

        // float that will be reset to 0 after hitting the attackCooldown variable
        [SerializeField] private float cooldown;

        [SerializeField]
        private CharacterBase characterBase; // the scriptable object that we fetch all the variables from

        [SerializeField] private float
            chasingSpeedMultiplier; // the multiplier for the movement speed of the enemy (1 if to move at same pace as the regular movement speed)

        [SerializeField] private int moveSpeed; // movement speed of the enemy
        [SerializeField] private float health;
        [SerializeField] public float maxHealth;
        [SerializeField] private float experienceRadius;

        [Header("Insert the level you want the enemy to be")] [SerializeField]
        private int level;

        [SerializeField] private float experience;
        [SerializeField] private new string name;
        private bool hasHealthBarShown;
        private Transform respawnParent;
        [SerializeField] private GameObject drop; // insert item to be dropped from prefab

        [SerializeField] private int dropChance; // insert value for dropchance
        private GameObject[] players;
        private int scale = 1;

        private void Start()
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
            maxHealth = characterBase.GetMaxHealth();
            // Runs for those who respawn
            PlayerScale();
            health = characterBase.GetMaxHealth();
        }

        public void Kill()
        {
            //TODO ADD EVENT LISTENER HERE, NEEDS TO FIND ALL LISTENERS FOR ENEMY DEATHS
            gameObject.SetActive(false);
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

        //get health back when moving back to default status
        public void BackToDefault()
        {
            this.gameObject.GetComponent<EnemyInfo>().health = maxHealth;
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
                float tempHealth = maxHealth;
                maxHealth = maxHealth * (float) Math.Pow(1.3, players.Length*1.45);
                damage = damage * (int) Math.Pow(1.3, players.Length*1.33);
                if (tempHealth == health)
                {
                    health = maxHealth;
                }
                gameObject.GetComponent<EnemyVitalController>().PlayerScaleHealthUpdate(health, maxHealth);
            }
        }
    }
}