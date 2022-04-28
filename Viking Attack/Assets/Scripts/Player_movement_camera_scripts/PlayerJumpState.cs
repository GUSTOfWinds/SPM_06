using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "PlayerState/JumpState")]
//Used as a state when the player presses "spacebar"
public class PlayerJumpState : PlayerState
{
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        if(Player.MyRigidbody3D.GroundedBool())
            Player.MyRigidbody3D.velocity += (Vector3.up * Player.jumpForce);

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
