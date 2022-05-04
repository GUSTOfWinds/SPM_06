using System.Collections;
using UnityEngine;

namespace ItemNamespace
{
    public class EnemyAttack : MonoBehaviour
    {
        /**
         * Animator stuff below
         */
        [SerializeField] private Animator animator;
        
        [SerializeField] private float range;  // The range of the enemy attacks
        [SerializeField] private float attackCooldown; // the cooldown of the enemy attacks
        [SerializeField] private int damage; // the damage of the enemy attacks
        [SerializeField] private float cooldown; // float that will be reset to 0 after hitting the attackCooldown variable
        [SerializeField] private CharacterBase characterBase; // the scriptable object that we fetch all the variables from
        [SerializeField] private GameObject player;
        [SerializeField] private GlobalPlayerInfo globalPlayerInfo;
        private Vector3 playerLocation; // location used to see if the player has gotten away far enough to not be hit
        private float playerUpdatedDistance; // location used to see if the player has gotten away far enough to not be hit
        private RaycastHit hit;
        

        void Start()
        {
            range = characterBase.GetRange();
            attackCooldown = characterBase.GetAttackCooldown();
            damage = characterBase.GetDamage();
        }
        
        private void FixedUpdate()
        {
            if (cooldown < attackCooldown) // adds to cooldown if attackCooldown hasn't been met
            {
                cooldown += Time.fixedDeltaTime;
            }

            
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 30))
            {
                // Prints a line of the raycast if a player is detected.
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance,
                    Color.yellow);
            
                // If in range and if cooldown has been passed and if the object that the raycast connects with has the tag Player.
                if (hit.distance < range && cooldown > attackCooldown && hit.collider.CompareTag("Player"))
                {
                    animator.SetBool("Chasing", false);
                    animator.SetBool("Attacking", true);
                    animator.SetBool("Patrolling", false);
                    gameObject.GetComponent<EnemyMovement>().attacking = true;
                    player = hit.collider.gameObject; // updates which player object to attack and to
                    globalPlayerInfo = player.GetComponent<GlobalPlayerInfo>();
                    StartCoroutine(FinishAttack());
                }
            }
        }

        private IEnumerator FinishAttack()
        {
            // saves the location of the player to be compared to the location at the impact
            playerLocation = player.transform.position; 
            ResetCoolDown(); // resets cooldown of the attack
            yield return new WaitForSeconds(1f);
            playerUpdatedDistance = Vector3.Distance (playerLocation, player.transform.position);
            if (playerUpdatedDistance < range)
            {
                Attack(); // Attacks player
            }
            gameObject.GetComponent<EnemyMovement>().attacking = false;
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
                globalPlayerInfo.UpdateHealth(-damage);
            }
        }
    }
}