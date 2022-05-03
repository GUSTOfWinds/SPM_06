using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCharacterScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterScreen;
    public void ToggleScreen()
    {
        if (characterScreen.active)
        {
            gameObject.GetComponent<ToggleMenu>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            characterScreen.SetActive(false);
            
        }
        else
        {
            gameObject.GetComponent<ToggleMenu>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            characterScreen.SetActive(true);
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}
