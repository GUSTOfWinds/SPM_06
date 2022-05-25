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
    public GameObject thisObject;
    public Animator animator;

    public GlobalPlayerInfo globalPlayerInfo;

    //KeyInfo variables  start
    public InputAction.CallbackContext movementKeyInfo;
    public InputAction.CallbackContext sprintKeyInfo;
    [HideInInspector] public bool jump;

    [SerializeField] private GameObject tutorialHandlerGameObject;
    private TutorialHandler tutorialHandler;
    
    public bool shouldMove;
    //KeyInfo variables  stop

    void Start()
    {
        tutorialHandler = tutorialHandlerGameObject.GetComponent<TutorialHandler>();
        thisObject = this.gameObject;
    }

    public override void OnStartAuthority()
    {
        shouldMove = true;
        myRigidbody = GetComponent<MyRigidbody3D>();
        if (states.Length > 0)
            stateMachine = new StateMachine(this, states);
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
    }


    void FixedUpdate()
    {
        //Cancels all updates that aren't the local player
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
        if (tutorialHandler.wasdFinished == false)
        {
            tutorialHandler.FinishWasd();
        }
        
        if (shouldMove)
        {
            movementKeyInfo = value;
        }
    }

    // Checks if the button for sprint is pressed, the value only functions as a bool (pressed or not) in the run state
    public void OnSprint(InputAction.CallbackContext value)
    {
        if (tutorialHandler.sprintFinished == false)
        {
            tutorialHandler.FinishSprint();
        }
        sprintKeyInfo = value;
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (tutorialHandler.dashFinished == false)
        {
            tutorialHandler.FinishDash();
        }
        
        jump = value.started;
        jump = !value.canceled;
    }
}