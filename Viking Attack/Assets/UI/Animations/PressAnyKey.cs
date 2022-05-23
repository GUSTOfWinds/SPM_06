using UnityEngine;
using UnityEngine.InputSystem;


public class PressAnyKey : MonoBehaviour
{
    public GameObject menu;
    public Animator animator;
    private UnityEngine.UI.Text text;
    
    void menutransition() 
    { 
        menu.SetActive(false);
    }

    public void AnyKey(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            animator.SetTrigger("pressedanykey");
            Invoke("menutransition", 1f);
        }
    }
}
