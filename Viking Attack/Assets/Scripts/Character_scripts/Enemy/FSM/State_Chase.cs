using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Chase : StateBase
{
    public State_Chase(Animator animator, GameObject gameObject, Vector3 spawnpos) : base(animator, gameObject, spawnpos)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("start Chasing");
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUppdate()
    {
        throw new System.NotImplementedException();
    }
}
