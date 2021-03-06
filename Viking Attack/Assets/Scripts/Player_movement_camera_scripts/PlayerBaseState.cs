using UnityEngine;


    [CreateAssetMenu(menuName = "PlayerState/BaseState")]
//Used as a state when the player does nothing
    public class PlayerBaseState : PlayerState
    {
        /*
            @Author Love Strignert - lost9373
        */
        public override void Enter()
        {
        
        }
        public override void Update()
        {
            // If the player is standing still, the stamina gets replenished.
            Player.globalPlayerInfo.UpdateStamina( Player.globalPlayerInfo.GetStaminaRegen() * Time.deltaTime);
        
            if(Player.movementKeyInfo.ReadValue<Vector2>() != Vector2.zero)
            {
                Animator.SetBool("isWalking",true);
                stateMachine.ChangeState<PlayerRunState>();
            }
        }
        public override void Exit()
        {
        
        }
    }

