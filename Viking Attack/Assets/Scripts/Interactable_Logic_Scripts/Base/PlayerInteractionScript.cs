using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


    public class PlayerInteractionScript : NetworkBehaviour
    {
        //The text that shows when hovering over an interactable object
        [SerializeField] private TextMeshProUGUI interactionText;

        private Camera mainCamera;

        private RaycastHit hit;

        public void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        }

        public void Update()
        {
            if (!isLocalPlayer) return;
            //Sends a raycast to check for colliders in the InteractableObject layer
            if (Physics.SphereCast(mainCamera.transform.position, 1f, mainCamera.transform.forward, out hit, 1,
                    LayerMask.GetMask("Enemy")))
            {
                //Changes text to the button and information that is set in the object hit
                interactionText.text = "Press: E to ";
                //Calls the function to say that the object is interacted with
            }
        //For check points **************
        if (Physics.SphereCast(mainCamera.transform.position, 1f, mainCamera.transform.forward, out hit, 1,
             LayerMask.GetMask("CheckPoint")))
        {
            //Changes text to the button and information that is set in the object hit
            Debug.Log("CheckPoint");
            hit.collider.gameObject.GetComponent<CheckPoint>().OnHit(gameObject);
            //Calls the function to say that the object is interacted with
        }
        //********************
        if (Physics.SphereCast(mainCamera.transform.position, 1f, mainCamera.transform.forward, out hit, 1,
                    LayerMask.GetMask("InteractableObject")))
            {
                //Changes text to the button and information that is set in the object hit
                interactionText.text = "Press: E to " +
                                       hit.transform.GetComponent<InteractableObjectScript>().InteractionDescription;
                //Calls the function to say that the object is interacted with
            }
            else
            {
                //Set text to nothing
                interactionText.text = "";
            }
        }

        public void OnInteraction(InputAction.CallbackContext value)
        {
            if (value.performed && hit.collider)
            {
                hit.transform.GetComponent<InteractableObjectScript>().ButtonPressed(gameObject);
            }
        }
    }
