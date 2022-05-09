//Base class to all player states

using UnityEngine;

public abstract class PlayerState : State
{
    private Animator animator;
    private PlayerScript3D player;
    //Returns player if player is null sets player veribal to owner veribal from State class 
    public PlayerScript3D Player => player = player ?? (PlayerScript3D)owner;
    public Animator Animator => animator = animator ?? player.animator;
}
