using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCharacterScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterScreen;
    
    public Animator animator;

    public void ToggleScreen()
    {
        if (animator.GetBool("CSOpen"))
        {
            animator.SetBool("CSOpen", false);
            if (gameObject.GetComponent<CameraMovement3D>().shouldBeLocked == false)
            {
                gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            animator.SetBool("CSOpen", true);
            Cursor.lockState = CursorLockMode.None;
            if (gameObject.GetComponent<CameraMovement3D>().shouldBeLocked == true)
            {
                gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = false;
            }
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}