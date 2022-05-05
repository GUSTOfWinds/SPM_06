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
        {   animator.SetBool("CSOpen", false);
/*            gameObject.GetComponent<ToggleMenu>().enabled = true;
*/            Cursor.lockState = CursorLockMode.Locked;
            
            
        }
        else
        {
            animator.SetBool("CSOpen", true);
/*            gameObject.GetComponent<ToggleMenu>().enabled = false;
*/            Cursor.lockState = CursorLockMode.None;
            

            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}
