using System.Collections;
using UnityEngine;

public class ItemSwordBehavior : ItemBaseBehavior
{
    private Animator animator;
    private Camera mainCamera = null;
    private GlobalPlayerInfo globalPlayerInfo;
    private RaycastHit hit;
    private bool canAttack = true;


    public void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
        animator = gameObject.transform.Find("Prefab_PlayerBot").GetComponent<Animator>();
    }
    public override void Use()
    {       
        // Checks if the player has enough stamina to attack, will then attack.
        if (globalPlayerInfo.GetStamina() > belongingTo.GetStamina && canAttack)
        {
            globalPlayerInfo.UpdateStamina(-belongingTo.GetStamina);
            // ADD SWING ANIMATION HERE
            canAttack = false;
            animator.Play("SwordAttack",animator.GetLayerIndex("Sword Attack"),0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"),1);
            StartCoroutine(WaitToAttack(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).length/animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).speed));
        }
    }
    IEnumerator WaitToAttack(float time)
    {

        yield return new WaitForSeconds(time);
        if(Physics.SphereCast(mainCamera.transform.position, 1f,mainCamera.transform.forward, out hit, belongingTo.GetRange,LayerMask.GetMask("Enemy")))
                hit.collider.gameObject.GetComponent<EnemyVitalController>().CmdUpdateHealth(-belongingTo.GetDamage);
        animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"),0);
        canAttack = true;
        
    }
}
        
    
