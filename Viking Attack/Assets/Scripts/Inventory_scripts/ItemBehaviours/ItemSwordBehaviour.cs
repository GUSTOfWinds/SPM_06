using System.Collections;
using Character_scripts.Enemy;
using Character_scripts.Player;
using Mirror;
using NewEnemyAiTest;
using UnityEngine;

namespace Inventory_scripts.ItemBehaviours
{
    public class ItemSwordBehaviour : ItemBaseBehaviour
    {
        private Animator animator;
        private GameObject rayCastPosition;
        private Camera mainCamera;
        private GlobalPlayerInfo globalPlayerInfo;
        private bool canAttack = true;
        public bool attackLocked;


        public void Awake()
        {
            rayCastPosition = gameObject.transform.Find("rayCastPosition").gameObject;
            mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
            globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
            animator = gameObject.transform.Find("Prefab_PlayerBot").GetComponent<Animator>();
        }

        // Might need some tweaking to work as we want
        IEnumerator AddAttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(1.5f);
            canAttack = true;
        }
        public override void Use(ItemBase itemBase)
        {
            // Checks if the player has enough stamina to attack, will then attack.
            if (globalPlayerInfo.GetStamina() > belongingTo.GetStamina && canAttack)
            {
                globalPlayerInfo.UpdateStamina(-belongingTo.GetStamina);

                canAttack = false;
                animator.Play("SwordAttack", animator.GetLayerIndex("Sword Attack"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 1);
                StartCoroutine(WaitToAttack(
                    animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).length /
                    animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).speed));
                if (globalPlayerInfo.GetStamina() < 15)
                {
                    StartCoroutine(AddAttackCooldown());
                }
            }
        }

        public override void StopAnimation()
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 0);
        }

        //Waits the length of the animation before letting the player attack again.
        IEnumerator WaitToAttack(float time)
        {
            // Used to lock the ability to swap between items while attacking
            attackLocked = true;
        
            yield return new WaitForSeconds(time / 2);
            Collider[] hits = Physics.OverlapSphere(rayCastPosition.transform.position, belongingTo.GetRange, LayerMask.GetMask("Enemy"));
            if (hits.Length > 0)
            {
                Collider hit = hits[0];
                // Damage on player now works as a multiplier instead of damage.
                hit.gameObject.GetComponent<EnemyVitalController>()
                    .CmdUpdateHealth(-(belongingTo.GetDamage * (globalPlayerInfo.GetDamage()) / 100), gameObject.GetComponent<NetworkIdentity>().netId);
                if (hit.gameObject.GetComponent<EnemyMovement>() != null)
                    hit.gameObject.GetComponent<EnemyMovement>().Stagger();
                else if (hit.gameObject.GetComponent<EnemyAIScript>() != null)
                    hit.gameObject.GetComponent<EnemyAIScript>().Stagger(1);
            }

            yield return new WaitForSeconds(time / 2);
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 0);
            canAttack = true;
        
            // Used to lock the ability to swap between items while attacking
            attackLocked = false;
        }
    }
}