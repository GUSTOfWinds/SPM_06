using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using TMPro;


public class EnemyHighligher : NetworkBehaviour
{
    //The text that shows when hovering over an interactable object


    private Camera mainCamera;

    private RaycastHit hit;
    private Material material;

    public void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
    }

    public void Update()
    {
        if (!isLocalPlayer) return;
        //Sends a raycast to check for colliders in the InteractableObject layer
        if (Physics.SphereCast(mainCamera.transform.position, 1f, mainCamera.transform.forward, out hit, 10,
                LayerMask.GetMask("Enemy")))
        {
            //Changes text to the button and information that is set in the object hit
            /*            hit.transform.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("Vector1_63645fd0daa5462ea5528c1ac3a77c0b", 1f);*/
            material = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
            material.SetFloat("Vector1_63645fd0daa5462ea5528c1ac3a77c0b", 1f);
            //Calls the function to say that the object is interacted with
        }


        else
        {
            //Set text to nothing
            material.SetFloat("Vector1_63645fd0daa5462ea5528c1ac3a77c0b", 0f);
        }
    }

}