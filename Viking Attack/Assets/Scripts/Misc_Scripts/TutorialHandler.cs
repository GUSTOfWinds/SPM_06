using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHandler : MonoBehaviour
{
    /*
        @Author Love Strignert - lost9373
    */
    private int whatTutorialLevel = 1;
    [SerializeField] private Animator animator;     
    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("tutorialstop"))
            Destroy(gameObject,0.5f);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        if(value.performed && whatTutorialLevel == 1)
        {
            animator.SetTrigger("checkmove");
            whatTutorialLevel++;
        }
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        if(value.performed && whatTutorialLevel == 2)
        {
            animator.SetTrigger("checksprint");
            whatTutorialLevel++;
        }
    }

    public void OnDash(InputAction.CallbackContext value)
    {
        if(value.performed && whatTutorialLevel == 3)
        {
            animator.SetTrigger("checkdash");
            whatTutorialLevel++;
        }
    }

    public void OnAttack(InputAction.CallbackContext value)
    {    
        if(value.performed && whatTutorialLevel == 4)
        {
            animator.SetTrigger("checkattack");
            whatTutorialLevel++;
        }
    }
    
    public void OnItemChange(InputAction.CallbackContext value)
    {    
        if(value.performed && whatTutorialLevel == 5)
        {
            animator.SetTrigger("checkitem");
            whatTutorialLevel++;
        }
    }

    public void OnExit(InputAction.CallbackContext value)
    {    
        if(value.performed)
        {
            Destroy(gameObject);
        }
    }
}
