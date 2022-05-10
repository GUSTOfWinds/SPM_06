using Mirror;
using UnityEngine;

//Base for all interactions
public abstract class BaseObjectInteraction : NetworkBehaviour
{
    virtual public void InteractedWith(GameObject playerThatInteracted){}
}
