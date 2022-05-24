using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Recall : StateBase
{
    [Header("inherit")]
    private Animator animator;
    private GameObject gameObject;
    private Vector3 spawnpos;
    private Enemy_FSM fsm;
    [Header("Own variabel")]
    private Vector3 movingDirection;
    private float moveSpeed;
    public State_Recall(Animator animator, GameObject gameObject, Vector3 spawnpos, Enemy_FSM fsm) : base(animator, gameObject, spawnpos, fsm)
    {
        this.animator = animator;
        this.gameObject = gameObject;
        this.spawnpos = spawnpos;
        this.fsm = fsm;
    }



    // Start is called before the first frame update
    public override void OnEnter()
    {
        Debug.Log("Recall On enter");
        moveSpeed = 2.5f;
    }

    public override void OnExit()
    {
        Debug.Log("Recall On Exit");
    }

    public override void OnUppdate()
    {
        //go back to the guarding area and change to guard state when close 
        gameObject.transform.LookAt(spawnpos);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, spawnpos, Time.fixedDeltaTime);

        movingDirection = (spawnpos - gameObject.transform.position);
        //Movement
        ChangeFacingDirection(movingDirection);

        gameObject.transform.position += movingDirection * moveSpeed * Time.fixedDeltaTime;
    }
    private void ChangeFacingDirection(Vector3 movingDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(movingDirection);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, 1);
    }
}
