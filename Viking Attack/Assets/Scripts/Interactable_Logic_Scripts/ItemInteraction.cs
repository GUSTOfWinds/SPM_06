using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : BaseObjectInteraction
{
    public override void InteractedWith(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerItemUsageController>().itemBase = this.gameObject.GetComponent<DropItemInWorldScript>().itembase;
        Destroy(this.gameObject);
    }
}
