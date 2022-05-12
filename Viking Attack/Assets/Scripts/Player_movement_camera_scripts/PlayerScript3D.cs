using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerScript3D : NetworkBehaviour
{
    [SerializeField] private State[] states;
    
    private MyRigidbody3D myRigidbody;
    private StateMachine stateMachine;
    public float acceleration = 12f;
    public bool firstPerson;
    
    public Animator animator;
    public GlobalPlayerInfo globalPlayerInfo;
    //KeyInfo variables  start
    public InputAction.CallbackContext movementKeyInfo;
    public InputAction.CallbackContext sprintKeyInfo;
    [HideInInspector] public bool jump;

    public bool shouldMove;
    //KeyInfo variables  stop
    
    public override void OnStartLocalPlayer()
    {
        shouldMove = true;
        myRigidbody = GetComponent<MyRigidbody3D>();
        if (states.Length > 0)
            stateMachine = new StateMachine(this, states);
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
    }


    void FixedUpdate()
    { //Cancels all updates that aren't the local player
        if (!LocalCheck()) return;
        //If there are any added states in the unity inspector
        if (states.Length > 0)
            stateMachine.Update();
    }

    public bool LocalCheck()
    {
        return isLocalPlayer;
    }
    //Returns myRigidbody
    public MyRigidbody3D MyRigidbody3D => myRigidbody;
    public void OnMovement(InputAction.CallbackContext value)
    {
        if (shouldMove)
        {
            movementKeyInfo = value;
        }
    }
    // Checks if the button for sprint is pressed, the value only functions as a bool (pressed or not) in the run state
    public void OnSprint(InputAction.CallbackContext value)
    {
        sprintKeyInfo = value;
    }
    public void OnJump(InputAction.CallbackContext value)
    {
        jump = value.started;
        jump = !value.canceled;  
    }
}