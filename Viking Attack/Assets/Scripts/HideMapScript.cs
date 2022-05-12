using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HideMapScript : MonoBehaviour
{
    [SerializeField] GameObject map;
    public void OnPress(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            if(map.activeSelf)
                map.SetActive(false);
            else
                map.SetActive(true);
        }
            
    }
}
