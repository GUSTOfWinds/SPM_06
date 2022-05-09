using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ItemNamespace;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Mirror;

public class EnemyMovement : NetworkBehaviour
{
    /**
     * Animation stuff below, to be merged with jiang
     */
    [SerializeField] private Animator animator;

    [SerializeField] public bool attacking;

    /**
     * 
     */
    [SerializeField] private int patrolRange;

    private Vector3 respawnPosWithoutY;
    private Rigidbody rigidBody;
    private Vector3 movingDirection;

    [Header("GroudCheck Settings")] [SerializeField]
    private GameObject groundCheck;

    private bool isGrounded;
    private LayerMask ground;
    private Collider[] colliders;

    [Header("Patrol Settnings")] [SerializeField]
    private float detectScopeRadius;

    private bool isGuarding;
    private bool isChasing;
    private bool backToDefault;
    private Vector3 posBeforeChasing; //save the position when enemy detected player
    private Collider[] sphereColliders;
    private GameObject chasingObject;

    [SerializeField] private float
        chasingSpeedMultiplier; // the multiplier for the movement speed of the enemy (1 if to move at same pace as the regular movement speed)

    [SerializeField] private int moveSpeed; // movement speed of the enemy
    [SerializeField] private CharacterBase characterBase; // the scriptable object that we fetch all the variables from

    // Syncs the position of the object to the server
    [SyncVar] [SerializeField] private Vector3 syncPosition;

    // Syncs the rotaion of the object to the server
    [SyncVar] [SerializeField] private Quaternion syncRotation;

    private Vector3 direction;

    private Vector3 facePlayer;

    private Quaternion lookRotation;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        chasingSpeedMultiplier = characterBase.GetChasingSpeed();
        moveSpeed = characterBase.GetMovementSpeed();
        isGuarding = true;
        ground = LayerMask.GetMask("Ground");
        movingDirection = Vector3.forward;
        var position = transform.position; // Enemy starting position 
        respawnPosWithoutY = new Vector3(position.x, position.y, position.z);
        transform.position = position;
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            colliders = Physics.OverlapBox(groundCheck.transform.position, new Vector3(0.1f, 0.1f, 0.1f),
                Quaternion.identity, ground); //Check if we are on the Ground
            if (colliders.Length > 0) //when we find the ground
            {
                isGrounded = true;
            }

            if (isGrounded) //start patrolling
            {
                if (isGuarding)
                {
                    animator.SetBool("Chasing", false);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Patrolling", true);
                    if (Vector3.Distance(transform.position, respawnPosWithoutY) >= patrolRange)
                    {
                        movingDirection = -movingDirection;
                    }

                    rigidBody.velocity = movingDirection * moveSpeed * Time.fixedDeltaTime;
                }

                if (isChasing)
                {
                    if (attacking)
                    {
                        return;
                    }
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Patrolling", false);
                    if (chasingObject.Equals(null)) return;
                    facePlayer = new Vector3(chasingObject.transform.position.x, transform.position.y,
                        chasingObject.transform.position.z);

                    direction = (chasingObject.transform.position - transform.position).normalized;
                    lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 3);
                    
                    if (Vector3.Distance(transform.position, chasingObject.transform.position) > 3f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, facePlayer,
                            chasingSpeedMultiplier * Time.fixedDeltaTime);
                    }
                }
                else
                {
                    CheckForPlayer();
                }
            }

            // if (backToDefault)
            // {
            //     animator.SetBool("Chasing", false);
            //     animator.SetBool("Attacking", false);
            //     animator.SetBool("Patrolling", true);
            //     if (Vector3.Distance(transform.position, respawnPosWithoutY) <= 3f)
            //     {
            //         backToDefault = false;
            //         isGuarding = true;
            //     }
            //     else
            //     {
            //         transform.position = Vector3.MoveTowards(transform.position, respawnPosWithoutY,
            //             chasingSpeedMultiplier * 1.5f * Time.deltaTime);
            //     }
            // }
            
            
            //Foljande 2 rader skickar ett kommando till servern och da andrar antingen positionen eller rotationen samt HP
            CmdSetSynchedPosition(transform.position);
            CmdSetSynchedRotation(transform.rotation);
        }
    }

    private void LateUpdate()
    {
        if (!isServer)
        {
            this.transform.position = syncPosition;
            this.transform.rotation = syncRotation;
        }
    }

    //Kommandlinjer for att be servern om uppdateringar po rotation och position
    [Command(requiresAuthority = false)]
    void CmdSetSynchedPosition(Vector3 position) => syncPosition = position;

    [Command(requiresAuthority = false)]
    void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;

    private void CheckForPlayer()
    {
        sphereColliders = Physics.OverlapSphere(transform.position, detectScopeRadius);
        foreach (var coll in sphereColliders)
        {
            if (coll.tag == "Player") //find Player and start chasing
            {
                posBeforeChasing = transform.position;
                chasingObject = coll.gameObject;
                isGuarding = false;
                isChasing = true;
            }
        }
    }
}