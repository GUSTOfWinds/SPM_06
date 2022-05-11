﻿using System.Collections;
using ItemNamespace;
using UnityEngine;
using Event;

public class ItemDaggerBehaviour : ItemBaseBehaviour
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
        }
    }
    
    //Waits the lenght of the animation before leting the player attack again.
    IEnumerator WaitToAttack(float time)
    {

        yield return new WaitForSeconds(time);
        if(Physics.SphereCast(rayCastPosition.transform.position, 0.1f,mainCamera.transform.forward, out hit, belongingTo.GetRange,LayerMask.GetMask("Enemy")))
        {
            hit.collider.gameObject.GetComponent<EnemyVitalController>().CmdUpdateHealth(-belongingTo.GetDamage);
            EnemyHitEvent hitEvent = new EnemyHitEvent();
            hitEvent.enemy = hit.collider.transform.gameObject;
            hitEvent.hitPoint = hit.point;

            EventSystem.Current.FireEvent(hitEvent);
        }
            
        animator.SetLayerWeight(animator.GetLayerIndex("Dagger Attack"),0);
        canAttack = true;
        
    }
}