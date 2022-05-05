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

            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            animator.SetBool("CSOpen", true);
            Cursor.lockState = CursorLockMode.None;
            
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}