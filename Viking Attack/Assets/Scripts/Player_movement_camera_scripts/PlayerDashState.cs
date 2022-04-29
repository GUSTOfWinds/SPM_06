using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "PlayerState/DashState")]
//Used as a state when the player presses "spacebar"
public class PlayerDashState : PlayerState
{
    private Vector3 input;
    private Vector2 inputMovement;
    public override void Enter()
    {
        
    }
    public override void Update()
    {

        inputMovement = Player.movementKeyInfo.ReadValue<Vector2>();
        input = new Vector3(inputMovement.x, 0, inputMovement.y);
        input = GameObject.FindGameObjectWithTag("CameraMain").transform.rotation * input; 
        input = Vector3.ProjectOnPlane(input, Player.MyRigidbody3D.Grounded().normal);
        input = input.normalized * Player.acceleration;
        
        // To compare and see what direction to move in in X and Z
        if (Math.Abs(Player.MyRigidbody3D.velocity.x) > Math.Abs(Player.MyRigidbody3D.velocity.z))
        {
            if (Player.MyRigidbody3D.velocity.x > 0)
            {
                Player.MyRigidbody3D.velocity.x += input.x + 20f;
            }
            else
            {
                Player.MyRigidbody3D.velocity.x += input.x - 20f;
            }
        }
        else if(Math.Abs(Player.MyRigidbody3D.velocity.x) > Math.Abs(Player.MyRigidbody3D.velocity.z))
        {
            if (Player.MyRigidbody3D.velocity.z > 0)
            {
                Player.MyRigidbody3D.velocity.z += input.z + 20f;
            }
            else
            {
                Player.MyRigidbody3D.velocity.z += input.z - 20f;
            }
        }
        

        if(Player.movementKeyInfo.ReadValue<Vector2>() != Vector2.zero)
            stateMachine.ChangeState<PlayerRunState>();
        else if(!Player.jump)
        {
            stateMachine.ChangeState<PlayerBaseState>();
        }
            
    }

    public override void Exit()
    {
        
    }
}
