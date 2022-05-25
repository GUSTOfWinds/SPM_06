using UnityEngine;

//Base class to all player states
public abstract class PlayerState : State
{
    /*
        @Author Love Strignert - lost9373
    */
    private Animator animator;
    private PlayerScript3D player;
    //Returns player if player is null sets player veribal to owner veribal from State class 
    public PlayerScript3D Player => player = player ?? (PlayerScript3D)owner;
    public Animator Animator => animator = animator ?? player.animator;
}

