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
    private Enemy_FSM fsm;

    protected StateBase(Animator animator, GameObject gameObject,Vector3 spawnpos,Enemy_FSM fsm)
    {
        this.animator = animator;
        this.gameObject = gameObject; 
        this.spawnpos = spawnpos;
        this.fsm = fsm;
    }

    public abstract void OnEnter();
    public abstract void OnUppdate();
    public abstract void OnExit();
    


}
