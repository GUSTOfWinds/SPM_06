using UnityEngine;
using UnityEngine.InputSystem;


public class PressAnyKey : MonoBehaviour
{
    public GameObject menu;
    public AudioSource audioSource;
    public AudioSource envAudioSource;
    public AudioSource seagullAudioSource;
    public AudioSource music;
    public AudioClip ac;
    
    public Animator animator;
    private UnityEngine.UI.Text text;
    private bool pressed = false;
    
    void menutransition() 
    { 
        menu.SetActive(false);
    }

    public void AnyKey(InputAction.CallbackContext value)
    {
        if (pressed == false)
        {
            if (value.performed)
            {
                audioSource.PlayOneShot(ac);
                envAudioSource.Play();
                seagullAudioSource.Play();
                music.Play();
                animator.SetTrigger("pressedanykey");
                Invoke("menutransition", 2.5f);
                pressed = true;
            }
        }
    }
}
