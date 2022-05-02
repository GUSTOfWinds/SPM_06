using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "PlayerState/DashState")]
//Used as a state when the player presses "spacebar"
public class PlayerDashState : PlayerState
{
    private Vector2 inputMovement;
    private Vector3 input;
    private float diff;

    public override void Enter()
    {
    }

    public override void Update()
    {
        // Calculates the input as we do in the run state.
        inputMovement = Player.movementKeyInfo.ReadValue<Vector2>();
        input = new Vector3(inputMovement.x, 0, inputMovement.y);
        input = GameObject.FindGameObjectWithTag("CameraMain").transform.rotation * input;
        input = Vector3.ProjectOnPlane(input, Player.MyRigidbody3D.Grounded().normal);

        // Checks that the player is moving, if so it will add 15f quickly to the current velocity
        if (Player.MyRigidbody3D.velocity.magnitude > 0.2f)
        {
            Player.MyRigidbody3D.velocity += input * 15f;
        }


        if (Player.movementKeyInfo.ReadValue<Vector2>() != Vector2.zero)
            stateMachine.ChangeState<PlayerRunState>();
        else if (!Player.jump)
        {
            stateMachine.ChangeState<PlayerBaseState>();
        }
    }

    public override void Exit()
    {
    }
}