using System;
using System.Collections;
using Character_scripts.Player;
using Event;
using Mirror;
using UnityEngine;

namespace Character_scripts.Enemy
{

    public class EnemyAttack : NetworkBehaviour
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] enemySounds;
        [SerializeField] private float range; // The range of the enemy attacks
        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks
        [SerializeField] private int damage; // the damage of the enemy attacks
        [SerializeField] private CharacterBase characterBase; // the scriptable object
        [SerializeField] private GameObject player;
        [SerializeField] private GlobalPlayerInfo globalPlayerInfo;
        private Vector3 playerLocation; // location used to see if the player has gotten away far enough to not be hit


        private float
            playerUpdatedDistance; // location used to see if the player has gotten away far enough to not be hit

        private RaycastHit hit;
        private Vector3 rayBeginning;

        [SerializeField]
        private float cooldown; // float that will be reset to 0 after hitting the attackCooldown variable

        private float cooldownSound = 0f;
        private float timeToNextSound = 6.5f;

        [SerializeField] private LayerMask layerMask;
        private EnemyMovement enemyMovement;
        private GameObject[] enemies;
        private Guid respawnEventGuid;

        [SerializeField] private DeathListener deathListener;
        //[SyncVar] private GameObject syncGlobalPlayerInfo;

        public IEnumerator finishAttack;

        void Start()
        {
            range = characterBase.GetRange();
            attackCooldown = characterBase.GetAttackCooldown();
            damage = characterBase.GetDamage();
            enemyMovement = gameObject.GetComponent<EnemyMovement>();
            if (isServer)
            {
                deathListener = FindObjectOfType<DeathListener>();
                enemies = deathListener.GetEnemies();
            }
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                if (cooldown < attackCooldown) // adds to cooldown if attackCooldown hasn't been met
                {
                    cooldown += Time.fixedDeltaTime;
                }

                if (cooldownSound < timeToNextSound) // adds to cooldown if attackCooldown hasn't been met
                {
                    cooldownSound += Time.fixedDeltaTime;
                }

                //if the enemy is stagger stop attack
                if (animator.GetBool("Staggered") && finishAttack != null)
                {
                    StopCoroutine(finishAttack);
                    enemyMovement.attacking = false;
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Patrolling", false);
                    return;
                }

                rayBeginning = transform.position;
                rayBeginning.y += 0.8f;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rayBeginning,
                        transform.TransformDirection(Vector3.forward), out hit, 7, layerMask))
                {
                    // Checks that no other enemies already are breathing within a 6 meter radius
                    if (!GetNearbyAudioSourcePlaying() && !audioSource.isPlaying && cooldownSound > timeToNextSound)
                    {
                        // plays the sound of the skeleton breathing when in range for attack
                        audioSource.PlayOneShot(enemySounds[0]);
                        RpcPlayEnemyChasing();
                        cooldownSound = 0;
                    }

                    // If in range and if cooldown has been passed and if the object that the raycast connects with has the tag Player.
                    if (hit.distance < range && cooldown > attackCooldown && enemyMovement.isAttacking)
                    {
                        // sets the animator of the enemy to Attacking
                        animator.SetBool("Attacking", true);
                        //sets the others to false
                        animator.SetBool("Chasing", false);
                        animator.SetBool("Patrolling", false);
                        player = hit.collider.gameObject; // updates which player object to attack and to
                        globalPlayerInfo = player.GetComponent<GlobalPlayerInfo>();
                        finishAttack = FinishAttack(hit.collider.gameObject);
                        StartCoroutine(finishAttack);
                    }
                }
            }
        }

        // Returns true if there is an enemy nearby already playing the chasing sound
        private bool GetNearbyAudioSourcePlaying()
        {
            enemies = deathListener.GetEnemies();
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) < 6f &&
                        enemy.GetComponent<AudioSource>().isPlaying && !enemy.Equals(gameObject))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerator FinishAttack(GameObject hitGo)
        {
            enemyMovement.attacking = true; // TODO REMOVE WHEN NEW MOVEMENT IS IN PLACE

            // saves the location of the player to be compared to the location at the impact
            playerLocation = player.transform.position;

            ResetCoolDown(); // resets cooldown of the attack


            
            yield return new WaitForSeconds(1f); // the time it takes from start of the enemy attack animation
            // to the time of impact, for smooth timing reasons
            // plays the sound of the skeleton swinging its sword
            RpcSwingSword();
            audioSource.PlayOneShot(enemySounds[1]);
            if (gameObject != null)
            {
                playerUpdatedDistance = Vector3.Distance(playerLocation, player.transform.position);
                if (playerUpdatedDistance < range)
                {
                    RpcDealDamage(hitGo);
                    Attack(); // Attacks player
                }

                animator.SetBool("Attacking", false);
                //sets the others to false
                animator.SetBool("Chasing", true);
                animator.SetBool("Patrolling", false);

                enemyMovement.attacking = false;
            }
        }

        [ClientRpc]
        private void RpcDealDamage(GameObject gpi)
        {
            if (isServer)
            {
                return;
            }

            // Armor here removes a portion of the damage 
            int tempDamage = damage - gpi.GetComponent<GlobalPlayerInfo>().GetArmorLevel();
            gpi.GetComponent<GlobalPlayerInfo>().UpdateHealth(-tempDamage);
        }

        [ClientRpc]
        private void RpcSwingSword()
        {
            if (isServer)
            {
                return;
            }

            audioSource.PlayOneShot(enemySounds[1]);
        }

        [ClientRpc]
        private void RpcPlayEnemyChasing()
        {
            if (isServer)
            {
                return;
            }

            audioSource.PlayOneShot(enemySounds[0]);
        }

        // Resets the attack cooldown
        private void ResetCoolDown()
        {
            cooldown = 0;
        }

        // Attacks with the damage of the object.
        private void Attack()
        {
            if (globalPlayerInfo.IsAlive()) // checks if the player is even alive
            {
                // Armor here removes a portion of the damage 
                int tempDamage = damage - globalPlayerInfo.GetArmorLevel();
                
                globalPlayerInfo.UpdateHealth(-tempDamage); // damages the player in question

                // Creates an event used to play a sound and display the damage in the player UI
                EventInfo playerDamageEventInfo = new PlayerDamageEventInfo
                {
                    EventUnitGo = gameObject,
                    target = player
                };
                EventSystem.Current.FireEvent(playerDamageEventInfo);
            }
        }
    }
}