using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[CreateAssetMenu(menuName = "PlayerState/RunState")]
//Used as a state when the player inputs for Horizontal and Vertical movement
public class PlayerRunState : PlayerState
{
    private Vector3 input;
    public override void Exit()
    {
        
    }
    public override void Update()
    {
        Vector2 inputMovement = Player.movementKeyInfo.ReadValue<Vector2>();
        input = new Vector3(inputMovement.x, 0, inputMovement.y);
        input = GameObject.FindGameObjectWithTag("CameraMain").transform.rotation * input; 
        input = Vector3.ProjectOnPlane(input, Player.MyRigidbody3D.Grounded().normal);
        input = input.normalized * Player.acceleration;

        //If player is grounded set input vector to follow the ground 
        if (!Player.MyRigidbody3D.GroundedBool())
            input = new Vector3(input.x, 0f, input.z);
        Player.MyRigidbody3D.velocity += input * Player.acceleration;

        if (Player.jump)
        {
            Player.jump = false;
            stateMachine.ChangeState<PlayerDashState>();
        }    
        else if (Player.movementKeyInfo.ReadValue<Vector2>() == Vector2.zero)
            stateMachine.ChangeState<PlayerBaseState>();
    }

}