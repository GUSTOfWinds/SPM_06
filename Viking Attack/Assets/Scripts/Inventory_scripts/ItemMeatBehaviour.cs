using System.Collections;
using Event;
using ItemNamespace;
using UnityEngine;

public class ItemMeatBehaviour : ItemBaseBehaviour
{
    private Animator animator;
    private GlobalPlayerInfo globalPlayerInfo;
    private PlayerInventory playerInventory;


    public void Awake()
    {
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
        animator = gameObject.transform.Find("Prefab_PlayerBot").GetComponent<Animator>();
        playerInventory = gameObject.GetComponent<PlayerInventory>();
    }

    // Will take the itembase heal ammount to heal the player if the player has any meat
    public override void Use(ItemBase itemBase)
    {
        // Checks if the player has enough food to eat, will then eat.
        if (globalPlayerInfo.GetMeatStackNumber() > 0 && globalPlayerInfo.GetHealth() < globalPlayerInfo.GetMaxHealth())
        {
            animator.Play("SwordAttack", animator.GetLayerIndex("Sword Attack"), 0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 1);
            StartCoroutine(WaitToEat(0.5f, itemBase));
        }
    }

    //Waits the lenght of the animation before leting the player attack again.
    IEnumerator WaitToEat(float time, ItemBase itemBase)
    {
        yield return new WaitForSeconds(time);
        // Creates an event used to play a sound and display the damage in the player UI
        EventInfo playerEatingEvent = new PlayerEatingEventInfo
        {
            EventUnitGo = gameObject
        };
        EventSystem.Current.FireEvent(playerEatingEvent);

        globalPlayerInfo.DecreaseMeatStackNumber();
        globalPlayerInfo.UpdateHealth(itemBase.GetHealAmount);
        playerInventory.UpdateMeatStack();
        animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 0);


        if (globalPlayerInfo.GetMeatStackNumber() == 0)
        {
            playerInventory.ReturnToDefault();
        }
    }
}