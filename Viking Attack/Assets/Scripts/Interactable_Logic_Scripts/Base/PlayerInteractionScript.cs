using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using TMPro;


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


        if(Physics.SphereCast(mainCamera.transform.position,0.8f, mainCamera.transform.forward,out hit,1,LayerMask.GetMask("InteractableObject")))
        {
            //Changes text to the button and information that is set in the object hit
            interactionText.text = "Press: E to " + hit.transform.GetComponent<InteractableObjectScript>().InteractionDescription;
            //Calls the function to say that the object is interacted with
        }else
        {
            //Set text to nothing
            interactionText.text = "";   
        }
    }

    public void OnInteraction(InputAction.CallbackContext value)    
    {
        if(value.performed && hit.collider)
        {
            hit.transform.GetComponent<InteractableObjectScript>().ButtonPressed(gameObject);
        }
    }
}
