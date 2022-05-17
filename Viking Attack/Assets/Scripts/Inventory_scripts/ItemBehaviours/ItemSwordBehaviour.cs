using System.Collections;
using ItemNamespace;
using UnityEngine;
using Event;

public class ItemSwordBehaviour : ItemBaseBehaviour
{
    private Animator animator;
    private GameObject rayCastPosition;
    private Camera mainCamera;
    private GlobalPlayerInfo globalPlayerInfo;
    private RaycastHit hit;
    private bool canAttack = true;


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

    //Waits the lenght of the animation before leting the player attack again.
    IEnumerator WaitToAttack(float time)
    {
        yield return new WaitForSeconds(time / 2);
        if (Physics.SphereCast(rayCastPosition.transform.position, 0.1f, mainCamera.transform.forward, out hit,
                belongingTo.GetRange, LayerMask.GetMask("Enemy")))
        {
            // Damage on player now works as a multiplier instead of damage.
            hit.collider.gameObject.GetComponent<EnemyVitalController>()
                .CmdUpdateHealth(-(belongingTo.GetDamage * (globalPlayerInfo.GetDamage()) / 100));
            if (hit.collider.gameObject.GetComponent<EnemyMovement>() != null)
                hit.collider.gameObject.GetComponent<EnemyMovement>().Stagger();
            else if (hit.collider.gameObject.GetComponent<EnemyAIScript>() != null)
                hit.collider.gameObject.GetComponent<EnemyAIScript>().Stagger();
            EnemyHitEvent hitEvent = new EnemyHitEvent();
            hitEvent.enemy = hit.collider.transform.gameObject;
            hitEvent.hitPoint = hit.point;

            EventSystem.Current.FireEvent(hitEvent);
        }
        if (Physics.SphereCast(rayCastPosition.transform.position, 0.1f, mainCamera.transform.forward, out hit,
               belongingTo.GetRange, LayerMask.GetMask("Breakable")))
        {

            yield return new WaitForSeconds(time / 2);
        animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 0);
        canAttack = true;
    }
}