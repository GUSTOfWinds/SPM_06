using System.Collections;
using ItemNamespace;
using UnityEngine;
using Event;

public class ItemSwordBehaviour : ItemBaseBehaviour
{
    private Animator animator;
    private Camera mainCamera = null;
    private GlobalPlayerInfo globalPlayerInfo;
    private bool canAttack = true;


    public void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
        animator = gameObject.transform.Find("Prefab_PlayerBot").GetComponent<Animator>();
    }
    public override void Use(ItemBase itemBase)
    {       
        // Checks if the player has enough stamina to attack, will then attack.
        if (globalPlayerInfo.GetStamina() > belongingTo.GetStamina && canAttack)
        {
            globalPlayerInfo.UpdateStamina(-belongingTo.GetStamina);

            canAttack = false;
            animator.Play("SwordAttack",animator.GetLayerIndex("Sword Attack"),0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"),1);
            StartCoroutine(WaitToAttack(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).length/animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Sword Attack")).speed));
        }
    }
    
    //Waits the lenght of the animation before leting the player attack again.
    IEnumerator WaitToAttack(float time)
    {

        yield return new WaitForSeconds(time);
        RaycastHit hit;
        if(Physics.SphereCast(mainCamera.transform.position, 1f,mainCamera.transform.forward, out hit, belongingTo.GetRange,LayerMask.GetMask("Enemy")))
        {
            hit.collider.gameObject.GetComponent<EnemyVitalController>().CmdUpdateHealth(-belongingTo.GetDamage);
            hit.collider.gameObject.GetComponent<EnemyMovement>().Stagger();
            EnemyHitEvent hitEvent = new EnemyHitEvent();
            hitEvent.enemy = hit.collider.transform.gameObject;
            hitEvent.hitPoint = hit.point;

            EventSystem.Current.FireEvent(hitEvent);
        }
                
        animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"),0);
        canAttack = true;

        
        
    }
    private void HitParticles()
    {
        
    }
}
        
    
