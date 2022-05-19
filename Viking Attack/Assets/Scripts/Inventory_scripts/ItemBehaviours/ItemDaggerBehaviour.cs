using System.Collections;
using ItemNamespace;
using UnityEngine;
using Event;
using Mirror;

public class ItemDaggerBehaviour : ItemBaseBehaviour
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
            animator.Play("Dagger_Attack",animator.GetLayerIndex("Dagger Attack"),0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Dagger Attack"),1);
            StartCoroutine(WaitToAttack(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Dagger Attack")).length/animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Dagger Attack")).speed));
            if (globalPlayerInfo.GetStamina() < 15)
            {
                StartCoroutine(AddAttackCooldown());
            }
        }
    }
    public override void StopAnimation()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Dagger Attack"),0);
    }
    
    //Waits the lenght of the animation before leting the player attack again.
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
        }
        yield return new WaitForSeconds(time / 2);
        animator.SetLayerWeight(animator.GetLayerIndex("Dagger Attack"),0);
        canAttack = true;
        
        // Used to lock the ability to swap between items while attacking
        attackLocked = false;
        
    }
}