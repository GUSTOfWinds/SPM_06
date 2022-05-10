using System.Collections;
using ItemNamespace;
using UnityEngine;

public class ItemMeatBehaviour : ItemBaseBehaviour
{
    private Animator animator;
    private Camera mainCamera = null;
    private GlobalPlayerInfo globalPlayerInfo;
    private RaycastHit hit;
    private bool canAttack = false;
    private PlayerInventory playerInventory;


    public void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
        animator = gameObject.transform.Find("Prefab_PlayerBot").GetComponent<Animator>();
        playerInventory = gameObject.GetComponent<PlayerInventory>();
    }

    // Will take the itembase heal ammount to heal the player if the player has any meat
    public override void Use(ItemBase itemBase)
    {
        if (globalPlayerInfo.GetMeatStackNumber() > 0)
        {
            // Checks if the player has enough stamina to attack, will then attack.
            animator.Play("SwordAttack", animator.GetLayerIndex("Sword Attack"), 0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Sword Attack"), 1);
            globalPlayerInfo.DecreaseMeatStackNumber();
            globalPlayerInfo.UpdateHealth(itemBase.GetHealAmount);
            playerInventory.UpdateMeatStack();
        }
    }
}