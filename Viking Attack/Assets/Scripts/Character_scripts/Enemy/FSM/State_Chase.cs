using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;

public class State_Chase : StateBase
{
    [Header("inherit")]
    private Animator animator;
    private GameObject gameObject;
    private Vector3 spawnpos;
    private Enemy_FSM fsm;

    [Header("Own variabel")]
    private GameObject chasingObject;
    private Vector3 movingDirection;
    private float chasingSpeed;
    private float attackRange;
    public State_Chase(Animator animator, GameObject gameObject, Vector3 spawnpos, Enemy_FSM fsm,GameObject chasingObject) : base(animator, gameObject, spawnpos, fsm)
    {
        this.animator = animator;
        this.gameObject = gameObject;
        this.spawnpos = spawnpos;
        this.fsm = fsm;
        this.chasingObject = chasingObject;
    }

    public override void OnEnter()
    {
        Debug.Log("start Chasing");
        chasingSpeed = 2f;
        attackRange = gameObject.GetComponent<EnemyInfo>().getAttckRange();
    }

    public override void OnExit()
    {
        Debug.Log("Stop chasing");
    }

    public override void OnUppdate()
    {
        movingDirection = (chasingObject.transform.position - gameObject.transform.position);
        //Movement
        ChangeFacingDirection(movingDirection);

        gameObject.transform.position +=  movingDirection * chasingSpeed * Time.fixedDeltaTime;
        if (Vector3.Distance(gameObject.transform.position, chasingObject.transform.position) <=
                   attackRange) //stop moving when player is in the attacking range 
        {
            movingDirection = Vector3.zero;
            Debug.Log("Should Attack now");
            //ATTACK
            
        }
        if(Vector3.Distance(gameObject.transform.position,spawnpos) >= 25f)
        {
            //if the enemy is chasing too far
            //Change to Recall state
           fsm.AddState(StateType.RECALL, new State_Recall(animator, this.gameObject, spawnpos,fsm));
           fsm.SetState(StateType.RECALL);
        }
    }
    private void ChangeFacingDirection(Vector3 movingDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(movingDirection);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, 1);
    }
}
