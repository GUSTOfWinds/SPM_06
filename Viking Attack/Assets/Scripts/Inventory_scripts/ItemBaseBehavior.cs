using UnityEngine;
using ItemNamespace;

public abstract class ItemBaseBehavior : MonoBehaviour
{
    protected ItemBase belongingTo;
    public abstract void Use();
    public void SetBelongingTo(ItemBase _belongingTo){belongingTo = _belongingTo;}
}
