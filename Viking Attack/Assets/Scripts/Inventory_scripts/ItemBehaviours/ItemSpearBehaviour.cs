﻿using System.Collections;
using ItemNamespace;
using UnityEngine;
using Event;

public class ItemSpearBehaviour : ItemBaseBehaviour
{
    private Animator animator;
    private GameObject rayCastPosition;
    private Camera mainCamera;
    private GlobalPlayerInfo globalPlayerInfo;
    private RaycastHit hit;
    private bool canAttack = true;
    public bool attackLocked;


    public void Awake()
    {
        rayCastPosition = gameObject.transform.Find("rayCastPosition").gameObject;
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
            animator.Play("Spear_Attack",animator.GetLayerIndex("Spear Attack"),0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Spear Attack"),1);
            StartCoroutine(WaitToAttack(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Spear Attack")).length/animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Spear Attack")).speed));
            if (globalPlayerInfo.GetStamina() < 15)
            {
                StartCoroutine(AddAttackCooldown());
            }
        }
    }
    
    // Might need some tweaking to work as we want
    IEnumerator AddAttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    public override void StopAnimation()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Spear Attack"), 0);
    }
    
    //Waits the length of the animation before letting the player attack again.
    IEnumerator WaitToAttack(float time)
    {
        // Used to lock the ability to swap between items while attacking
        attackLocked = true;

        yield return new WaitForSeconds(time / 2);
        if(Physics.SphereCast(rayCastPosition.transform.position, 0.1f,mainCamera.transform.forward, out hit, belongingTo.GetRange,LayerMask.GetMask("Enemy")))
        {
            hit.collider.gameObject.GetComponent<EnemyVitalController>().CmdUpdateHealth(-(belongingTo.GetDamage * (globalPlayerInfo.GetDamage()) / 100));
            if(hit.collider.gameObject.GetComponent<EnemyMovement>() != null)
                hit.collider.gameObject.GetComponent<EnemyMovement>().Stagger();
            else if(hit.collider.gameObject.GetComponent<EnemyAIScript>() != null)
                hit.collider.gameObject.GetComponent<EnemyAIScript>().Stagger();
            EnemyHitEvent hitEvent = new EnemyHitEvent();
            hitEvent.enemy = hit.collider.transform.gameObject;
            hitEvent.hitPoint = hit.point;

            EventSystem.Current.FireEvent(hitEvent);
        }
        yield return new WaitForSeconds(time / 2);
        animator.SetLayerWeight(animator.GetLayerIndex("Spear Attack"),0);
        canAttack = true;

        // Used to lock the ability to swap between items while attacking
        attackLocked = false;
    }
}