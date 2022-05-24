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
    private Vector3 spawnpos;

    protected StateBase(Animator animator, GameObject gameObject,Vector3 spawnpos)
    {
        this.animator = animator;
        this.gameObject = gameObject; 
        this.spawnpos = spawnpos;
    }

    public abstract void OnEnter();
    public abstract void OnUppdate();
    public abstract void OnExit();
  


}
