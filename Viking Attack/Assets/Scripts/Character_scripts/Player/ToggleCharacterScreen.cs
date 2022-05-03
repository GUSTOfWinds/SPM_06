using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCharacterScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterScreen;
    public Animator animator;
    //[SerializeField] private InputSystem inputSystem;
    public void ToggleScreen()
    {
        if (animator.GetBool("CSSheetOpen"))
        {
            animator.SetBool("CSSheetOpen", false);
            Cursor.lockState = CursorLockMode.Locked;
            
            
        }
        else
        {
            animator.SetBool("CSSheetOpen", true);            
            Cursor.lockState = CursorLockMode.None;
            
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}
