using System.Collections;
using System.Collections.Generic;
using ItemNamespace;
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
    private float sprintCost = 7f;
    private float staminaGain = 12f;
    [SerializeField] private float cooldown = 0.9f;

    public override void Exit()
    {
        
    }
    public override void FixedUpdate()
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
            Animator.SetBool("isRunning",true);
            Animator.SetBool("isWalking",false);
            Player.globalPlayerInfo.UpdateStamina( -sprintCost * Time.deltaTime);
            input = input.normalized * Player.acceleration * 1.6f;
        }
        else
        {
            Animator.SetBool("isRunning",false);
            Animator.SetBool("isWalking",true);
            Player.globalPlayerInfo.UpdateStamina( staminaGain * Time.deltaTime);
            input = input.normalized * Player.acceleration;
        }

        
        
        
        //If player is grounded set input vector to follow the ground 
        if (!Player.MyRigidbody3D.GroundedBool())
            input = new Vector3(input.x, 0f, input.z);
        Player.MyRigidbody3D.velocity += input * Player.acceleration;

        cooldown -= Time.deltaTime;
        if (cooldown <= 0.0f && Player.jump)
        {
            Animator.SetBool("isRunning",false);
            Animator.SetBool("isWalking",false);
            Player.jump = false;
            stateMachine.ChangeState<PlayerDashState>();
            cooldown = 0.9f;
        }
        
        else if (Player.movementKeyInfo.ReadValue<Vector2>() == Vector2.zero)
        {
            Animator.SetBool("isRunning",false);
            Animator.SetBool("isWalking",false);
            stateMachine.ChangeState<PlayerBaseState>();
        }
            
    }

}