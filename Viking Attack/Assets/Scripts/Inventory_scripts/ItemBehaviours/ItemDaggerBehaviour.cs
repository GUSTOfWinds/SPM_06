using System.Collections;
using UnityEngine;
using ItemNamespace;
using Mirror;

public class ItemDaggerBehaviour : ItemBaseBehaviour
{
    /*
        @Author Love Strignert - lost9373
    */
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
        animator = gameObject.transform.Find("VikingWarrior").GetComponent<Animator>();
    }

    // Might need some tweaking to work as we want
    IEnumerator AddAttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1f);
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
        Collider[] hits = Physics.OverlapSphere(rayCastPosition.transform.position + mainCamera.transform.forward*belongingTo.GetRange/2, belongingTo.GetRange/2, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            Collider enemy = null;
            float closest = Mathf.Infinity;
            foreach(Collider hit in hits)
            {
                if(Vector3.Distance(hit.transform.position, gameObject.transform.position) < closest)
                {
                    closest = Vector3.Distance(hit.transform.position, gameObject.transform.position);
                    enemy = hit;
                } 
            }
            // Damage on player now works as a multiplier instead of damage.
            float damage = -(belongingTo.GetDamage * (globalPlayerInfo.GetDamage()) / 100);
            if(enemy.GetComponent<EnemyInfo>().GetCharacterBase().GetEnemyType() == CharacterBase.EnemyType.Skeleton)
            {
                damage -= 5;
                GameObject temp = Instantiate(belongingTo.GetParticle,enemy.gameObject.transform.position + new Vector3(0,2,0),enemy.gameObject.transform.rotation,enemy.gameObject.transform);
                temp.transform.localScale = enemy.gameObject.transform.localScale;
                Destroy(temp,0.4f);
            }
                
            enemy.gameObject.GetComponent<EnemyVitalController>().CmdUpdateHealth(damage, gameObject.GetComponent<NetworkIdentity>().netId);
        }
        Collider[] hitBreakable = Physics.OverlapSphere(rayCastPosition.transform.position, belongingTo.GetRange, LayerMask.GetMask("Breakable"));
        if (hitBreakable.Length > 0)
        {
            Collider hit = hitBreakable[0];
            hit.gameObject.GetComponent<BreakableBehavior>().Break();
        }

        yield return new WaitForSeconds(time / 2);
        animator.SetLayerWeight(animator.GetLayerIndex("Dagger Attack"),0);
        canAttack = true;
        
        // Used to lock the ability to swap between items while attacking
        attackLocked = false;
        
    }
}