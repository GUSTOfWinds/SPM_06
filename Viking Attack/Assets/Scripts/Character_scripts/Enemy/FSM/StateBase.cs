using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//super class of all state
public abstract class StateBase 
{
    //since we need to use animation but we dont use MonoBehivor
    //We can call it when we instantiate the state
    private Animator animator;
    private GameObject gameObject;

    protected StateBase(Animator animator, GameObject gameObject)
    {
        this.animator = animator;
        this.gameObject = gameObject; 
    }

    public abstract void OnEnter();
    public abstract void OnUppdate();
    public abstract void OnExit();
  


}
