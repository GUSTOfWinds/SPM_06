using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FSM_Tst : NetworkBehaviour
{
    [SerializeField] private GameObject groundCheck;
    private Enemy_FSM Enemy_FSM;
    private Animator animator;
    private Vector3 spawnPs;
    private Collider[] colliders;
    private LayerMask ground;
    
    private bool isGrounded;
   

    private void Awake()
    {
       Enemy_FSM = new Enemy_FSM();
       
        //***********//
        ground = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        
        if (isServer)
        {
            colliders = Physics.OverlapBox(groundCheck.transform.position, new Vector3(0.1f, 0.1f, 0.1f),
    Quaternion.identity, ground); //Check if we are on the Ground

            if (colliders.Length > 0) //when we find the ground
            {
                isGrounded = true;

            }
            if (isGrounded)
            {
                Enemy_FSM.AddState(StateType.GUARD, new State_Guard(animator, this.gameObject, transform.position));
                Enemy_FSM.SetState(StateType.GUARD);
                Enemy_FSM.OnTick();
            }
        }
    }

    public void getSpwnPos(Transform tr)
    {
        spawnPs = tr.position;
    }
}
