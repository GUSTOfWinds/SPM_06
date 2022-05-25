using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Event;
using Inventory_scripts;
using ItemNamespace;
using UnityEngine;

public class ItemMeatBehaviour : ItemBaseBehaviour
{
    /**
     * @author Martin Kings
     */
    private Animator animator;

    private GlobalPlayerInfo globalPlayerInfo;
    private PlayerInventory playerInventory;
    public bool eating;


    public void Awake()
    {
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
        animator = gameObject.transform.Find("VikingWarrior").GetComponent<Animator>();
        playerInventory = gameObject.GetComponent<PlayerInventory>();
    }

    // Will take the itembase heal ammount to heal the player if the player has any meat
    public override void Use(ItemBase itemBase)
    {
        // Checks if the player has enough food to eat and has missing HP will then eat.
        if (globalPlayerInfo.GetMeatStackNumber() > 0 && globalPlayerInfo.GetHealth() < globalPlayerInfo.GetMaxHealth())
        {
            if (eating)
            {
                return;
            }
            
            animator.Play("Eat Meat", animator.GetLayerIndex("Eat Meat"), 0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Eat Meat"), 1);
            StartCoroutine(WaitToEat(
                animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Eat Meat")).length /
                animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Eat Meat")).speed, itemBase));
            
        }
    }

    public override void StopAnimation()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Eat Meat"), 0);
    }

    IEnumerator WaitToEat(float time, ItemBase itemBase)
    {
        eating = true;
        // Creates an event used to play a sound and display the damage in the player UI
        EventInfo playerEatingEvent = new PlayerEatingEventInfo
        {
            EventUnitGo = gameObject
        };
        EventSystem.Current.FireEvent(playerEatingEvent);
        yield return new WaitForSeconds(time);

        globalPlayerInfo.DecreaseMeatStackNumber();
        globalPlayerInfo.UpdateHealth(itemBase.GetHealAmount);
        playerInventory.UpdateMeatStack();
        animator.SetLayerWeight(animator.GetLayerIndex("Eat Meat"), 0);


        if (globalPlayerInfo.GetMeatStackNumber() == 0)
        {
            playerInventory.ReturnToDefault();
        }
        eating = false;
    }
}