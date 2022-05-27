using Mirror;
using UnityEngine;

//Base for all interactions

    public abstract class BaseObjectInteraction : NetworkBehaviour
    {
        /**
         * @Author Love Strignert - lost9373
        */
        virtual public void InteractedWith(GameObject playerThatInteracted){}
    }

