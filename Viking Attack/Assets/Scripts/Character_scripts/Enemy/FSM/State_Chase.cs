using UnityEngine;

public class State_Chase : StateBase
{
    public State_Chase(Animator animator, GameObject gameObject) : base(animator, gameObject)
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
