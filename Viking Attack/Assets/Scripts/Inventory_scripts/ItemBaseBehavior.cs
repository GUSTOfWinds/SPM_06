using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;

//This is the base behavior for all items, used ot give different items different behaviors
public abstract class ItemBaseBehavior : MonoBehaviour
{
    protected ItemBase belongingTo;
    public abstract void Use();
    public void SetBelongingTo(ItemBase _belongingTo){belongingTo = _belongingTo;}
}
