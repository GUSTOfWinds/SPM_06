using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCharacterScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterScreen;
    //[SerializeField] private InputSystem inputSystem;
    public void ToggleScreen()
    {
        if (characterScreen.active)
        {
            Cursor.lockState = CursorLockMode.Locked;
            characterScreen.SetActive(false);
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            characterScreen.SetActive(true);
            characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        }
    }
}
