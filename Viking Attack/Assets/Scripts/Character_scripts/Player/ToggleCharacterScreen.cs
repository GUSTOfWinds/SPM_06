using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCharacterScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterScreen;
<<<<<<< HEAD
=======
    public Animator animator;
    //[SerializeField] private InputSystem inputSystem;
>>>>>>> parent of d094a905 (Revert "Ny HP, Stamina och hotbar. WIP. Animationer för character screen. Finare knappar.")
    public void ToggleScreen()
    {
        if (animator.GetBool("CSSheetOpen"))
        {
<<<<<<< HEAD
            gameObject.GetComponent<ToggleMenu>().enabled = true;
=======
            animator.SetBool("CSSheetOpen", false);
>>>>>>> parent of d094a905 (Revert "Ny HP, Stamina och hotbar. WIP. Animationer för character screen. Finare knappar.")
            Cursor.lockState = CursorLockMode.Locked;
            
            
        }
        else
        {
<<<<<<< HEAD
            gameObject.GetComponent<ToggleMenu>().enabled = false;
=======
            animator.SetBool("CSSheetOpen", true);            
>>>>>>> parent of d094a905 (Revert "Ny HP, Stamina och hotbar. WIP. Animationer för character screen. Finare knappar.")
            Cursor.lockState = CursorLockMode.None;
            
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}
