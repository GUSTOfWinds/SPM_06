using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;

public class ItemSwordBehavior : ItemBaseBehavior
{
    private Camera mainCamera = null;
    private GlobalPlayerInfo globalPlayerInfo;
    private RaycastHit hit;


    public void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
    }
    public override void Use()
    {       
        if (Physics.SphereCast(mainCamera.transform.position, 1f,mainCamera.transform.forward, out hit, belongingTo.GetRange,LayerMask.GetMask("Enemy")))
        {
            // Checks if the player has enough stamina to attack, will then attack.
            if (globalPlayerInfo.GetStamina() > belongingTo.GetStamina)
            {
                globalPlayerInfo.UpdateStamina(-belongingTo.GetStamina);
                hit.collider.gameObject.GetComponent<EnemyVitalController>().UpdateHealth(-belongingTo.GetDamage);
                // ADD SWING ANIMATION HERE
            }
        }
    }
}
