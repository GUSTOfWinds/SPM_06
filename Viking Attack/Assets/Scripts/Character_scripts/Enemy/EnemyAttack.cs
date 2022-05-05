using System;
using System.Collections;
using Event;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ItemNamespace
{
    public class EnemyAttack : MonoBehaviour
    {
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

        [SerializeField] private LayerMask layerMask;
        private EnemyMovement enemyMovement;
        private GameObject[] enemies;
        private Guid respawnEventGuid;


        void Start()
        {
            range = characterBase.GetRange();
            attackCooldown = characterBase.GetAttackCooldown();
            damage = characterBase.GetDamage();
            enemyMovement = gameObject.GetComponent<EnemyMovement>();
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            EventSystem.Current.RegisterListener<EnemyRespawnEventInfo>(OnEnemyRespawn, ref respawnEventGuid);
        }

        public void BeforeAttack()
        {
            if (cooldown < attackCooldown) // adds to cooldown if attackCooldown hasn't been met
            {
                cooldown += Time.fixedDeltaTime;
            }

            rayBeginning = transform.position;
            rayBeginning.y += 0.8f;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(rayBeginning,
                    transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
            {
                // Checks that no other enemies already are breathing within a 6 meter radius
                if (!GetNearbyAudioSourcePlaying() && !audioSource.isPlaying)
                {
                    // plays the sound of the skeleton breathing when in range for attack
                    audioSource.PlayOneShot(enemySounds[0]);
                }

                ;


                // If in range and if cooldown has been passed and if the object that the raycast connects with has the tag Player.
                if (hit.distance < range && cooldown > attackCooldown)
                {
                    animator.SetBool("Chasing", false);
                    animator.SetBool("Attacking", true);
                    animator.SetBool("Patrolling", false);
                    enemyMovement.attacking = true; // TODO REMOVE WHEN NEW MOVEMENT IS IN PLACE
                    player = hit.collider.gameObject; // updates which player object to attack and to
                    globalPlayerInfo = player.GetComponent<GlobalPlayerInfo>();
                    StartCoroutine(FinishAttack());
                }
            }
        }

        // Returns true if there is an enemy nearby already playing the chasing sound
        private bool GetNearbyAudioSourcePlaying()
        {
            foreach (var enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) < 6f) ;
                {
                    if (enemy.GetComponent<AudioSource>().isPlaying)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerator FinishAttack()
        {
            // saves the location of the player to be compared to the location at the impact
            playerLocation = player.transform.position;

            ResetCoolDown(); // resets cooldown of the attack

            // plays the sound of the skeleton swinging its sword
            audioSource.PlayOneShot(enemySounds[1]);

            yield return new WaitForSeconds(1f); // the time it takes from start of the enemy attack animation
            // to the time of impact, for smooth timing reasons

            playerUpdatedDistance = Vector3.Distance(playerLocation, player.transform.position);
            if (playerUpdatedDistance < range)
            {
                Attack(); // Attacks player
            }

            enemyMovement.attacking = false;
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
                globalPlayerInfo.UpdateHealth(-damage); // damages the player in question

                // Creates an event used to play a sound and display the damage in the player UI
                EventInfo playerDamageEventInfo = new DamageEventInfo
                {
                    EventUnitGo = gameObject,
                    EventDescription = "Unit " + gameObject.name + " has died.",
                    target = player
                };
                EventSystem.Current.FireEvent(playerDamageEventInfo);
            }
        }

        // Refreshes the array of enemies upon the time an enemy respawns
        private void OnEnemyRespawn(EnemyRespawnEventInfo enemyRespawnEventInfo)
        {
            Debug.Log("NU RESPAWNAR EN ENEMY");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        public void UnregisterRespawnListener()
        {
            Debug.Log("UNREGISTRAS NU");
            EventSystem.Current.UnregisterListener(respawnEventGuid);
        }
    }
}