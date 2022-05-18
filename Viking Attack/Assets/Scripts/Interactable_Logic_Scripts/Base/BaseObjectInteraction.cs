using Mirror;
using UnityEngine;

//Base for all interactions
namespace Interactable_Logic_Scripts.Base
{
    public abstract class BaseObjectInteraction : NetworkBehaviour
    {
        virtual public void InteractedWith(GameObject playerThatInteracted){}
    }
}
