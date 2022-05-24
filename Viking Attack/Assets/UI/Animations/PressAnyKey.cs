using UnityEngine;
using UnityEngine.InputSystem;


public class PressAnyKey : MonoBehaviour
{
    public GameObject menu;
    public AudioSource audioSource;
    public AudioSource envAudioSource;
    public AudioSource seagullAudioSource;
    public AudioClip ac;
    
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
            audioSource.PlayOneShot(ac);
            envAudioSource.Play();
            seagullAudioSource.Play();
            animator.SetTrigger("pressedanykey");
            Invoke("menutransition", 2.5f);
        }
    }
}
