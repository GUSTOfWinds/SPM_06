using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(menuName = "PlayerState/BaseState")]
//Used as a state when the player does nothing
public class PlayerBaseState : PlayerState
{

    public override void Enter()
    {
        
    }
    public override void Update()
    {
        // If the player is standing still, the stamina gets replenished.
        Player.globalPlayerInfo.UpdateStamina( 17f * Time.deltaTime);
        
        
        if(Player.movementKeyInfo.ReadValue<Vector2>() != Vector2.zero)
        {
            stateMachine.ChangeState<PlayerRunState>();
        }
    }
    public override void Exit()
    {
        
    }
}
