using UnityEngine;


    public class InteractableObjectScript : MonoBehaviour
    {
        /**
         * @Author Love Strignert - lost9373
        */
        [SerializeField] private string interactionDescriptionPositiv; //The text with information about a uninteracted object
        [SerializeField] private string interactionDescriptionNegative; //The text with information about a interacted object
        [SerializeField] private GameObject interactableGameObject; //The object that is interacted with
        private string interactionDescription;
        public string InteractionDescription => interactionDescription;
        public void Start()
        {
            interactionDescription = interactionDescriptionPositiv;
        }
        public void ButtonPressed(GameObject playerThatInteracted)
        {
            if(interactionDescription.Equals(interactionDescriptionPositiv))
                interactionDescription = interactionDescriptionNegative;
            else
                interactionDescription = interactionDescriptionPositiv;
            //Calls the object to interact with (uses the BaseObjectInteraction so i can call different objects)
            interactableGameObject.GetComponent<BaseObjectInteraction>().InteractedWith(playerThatInteracted);
        }
    }

