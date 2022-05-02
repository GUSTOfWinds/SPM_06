using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[CreateAssetMenu(menuName = "PlayerState/RunState")]
//Used as a state when the player inputs for Horizontal and Vertical movement
public class PlayerRunState : PlayerState
{
    private Vector3 input;
    private Vector2 inputMovement;
    public InputAction.CallbackContext sprintKeyInfo;
    private float sprintCost = 8f;
    private float staminaGain = 5f;
    
    
    public override void Exit()
    {
        
    }
    public override void Update()
    {
        inputMovement = Player.movementKeyInfo.ReadValue<Vector2>();
        sprintKeyInfo = Player.sprintKeyInfo;
        
        input = new Vector3(inputMovement.x, 0, inputMovement.y);
        input = GameObject.FindGameObjectWithTag("CameraMain").transform.rotation * input; 
        input = Vector3.ProjectOnPlane(input, Player.MyRigidbody3D.Grounded().normal);
        // if the sprintkey is held and the player has stamina enough to run, the stamina
        // decreases by the sprintCost, else the player regains stamina while walking
        if (sprintKeyInfo.performed && Player.globalPlayerInfo.GetStamina() > 1)
        {
            Player.globalPlayerInfo.UpdateStamina( -sprintCost * Time.deltaTime);
            input = input.normalized * Player.acceleration * 2;
        }
        else
        {
            Player.globalPlayerInfo.UpdateStamina( staminaGain * Time.deltaTime);
            input = input.normalized * Player.acceleration;
        }

        
        
        
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