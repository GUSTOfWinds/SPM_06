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
    private int hitsForStagger = 0;

    /**
     * 
     */
    [Header("Base Movement")] private Vector3 movingDirection;

    private float waitFrame;
    private float defaultSpeed;
    private Vector3 point1, point2;
    private CapsuleCollider capsCollider;

    [Header("GroudCheck Settings")] [SerializeField]
    private GameObject groundCheck;

    private bool isGrounded;
    private LayerMask ground;
    private LayerMask player;
    private Collider[] colliders;
    private float changeTimer;

    [Header("State Boolean")] 
    private bool isGuarding;
    private bool isChasing;
    public bool isAttacking;
    private bool backToDefault;
    private bool checkForHinder;
    private bool canChange;

    [Header("Patrol Settnings")] private Collider[] sphereColliders;
    private GameObject chasingObject;
    private Vector3 spawnPosition;
    private float attackRange;
    [SerializeField] private float detectScopeRadius;
    [SerializeField] private float patrolRange;
    [SerializeField] private int maxChasingRange;

    [SerializeField] private float
        chasingSpeedMultiplier; // the multiplier for the movement speed of the enemy (1 if to move at same pace as the regular movement speed)

    [SerializeField] private float moveSpeed; // movement speed of the enemy
    [SerializeField] private CharacterBase characterBase; // the scriptable object that we fetch all the variables from

    [Header("Calculation")] private int traces = 6;
    private float visionAngle = 45.0f;
    private Vector3 positionTemp;

    // Syncs the position of the object to the server
    [SyncVar] [SerializeField] private Vector3 syncPosition;

    // Syncs the rotaion of the object to the server
    [SyncVar] [SerializeField] private Quaternion syncRotation;

    void Start()
    {
        chasingSpeedMultiplier = characterBase.GetChasingSpeed();
        defaultSpeed = characterBase.GetMovementSpeed();
        attackRange = characterBase.GetRange();
        moveSpeed = defaultSpeed;
        isGuarding = true;
        canChange = false;
        changeTimer = 0;
        ground = LayerMask.GetMask("Ground");
        player = LayerMask.GetMask("Player");

        movingDirection = RandomVector(-patrolRange, patrolRange);

        waitFrame = 0;

        capsCollider = GetComponent<CapsuleCollider>();
        point1 = gameObject.transform.position + capsCollider.center +
                 Vector3.up * (capsCollider.height / 2 - capsCollider.radius);
        point2 = gameObject.transform.position + capsCollider.center +
                 Vector3.down * (capsCollider.height / 2 - capsCollider.radius);
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("Staggered"))
        {
            return;
        }

        //change the transform.position from botten to central 
        positionTemp = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        if (attacking)
        {
            return;
        }

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
                moveSpeed = defaultSpeed;
                checkForHinder = true;
                if (isGuarding)
                {
                    animator.SetBool("Chasing", false);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Patrolling", true);

                    if (Vector3.Distance(transform.position, spawnPosition) >=
                        patrolRange) //when enemy is moving too far, change moving direction
                    {
                        if (canChange)
                        {
                            movingDirection = RandomVector(movingDirection).normalized;
                            checkForHinder = false;
                            canChange = false;
                            changeTimer = 0;
                        }
                        
                    }
                    changeTimer += Time.fixedDeltaTime;
                    if(changeTimer >= 1f)
                    {
                        canChange = true;
                    }

                    transform.position += 0.3f * movingDirection * moveSpeed * Time.fixedDeltaTime;
                    ChangeFacingDirection(movingDirection);
                }

                if (isChasing)
                {
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Patrolling", false);
                    backToDefault = false;
                    //Vector3 facePlayer = new Vector3(chasingObject.transform.position.x, transform.position.y, chasingObject.transform.position.z);
                    movingDirection = (chasingObject.transform.position - transform.position);
                    //Movement
                    ChangeFacingDirection(movingDirection);

                    transform.position += 0.1f * movingDirection * moveSpeed * Time.fixedDeltaTime;

                    if (Vector3.Distance(transform.position, chasingObject.transform.position) <=
                        attackRange) //stop moving when player is in the attacking range 
                    {
                        movingDirection = Vector3.zero;
                        //ATTACK
                        isAttacking = true;
                        isChasing = false;
                    }

                    if (Vector3.Distance(transform.position, spawnPosition) >= maxChasingRange)
                    {
                        isChasing = false;
                        backToDefault = true;
                    }
                } //is chasing


                if (isAttacking)
                {
                    backToDefault = false;
                    transform.LookAt(chasingObject.transform.position);
                    if (chasingObject != null)
                    {
                        if (Vector3.Distance(transform.position, spawnPosition) >= maxChasingRange)
                        {
                            isAttacking = false;
                            backToDefault = true;
                        }

                        if (Vector3.Distance(transform.position, chasingObject.transform.position) >= 5f &&
                            chasingObject.GetComponent<GlobalPlayerInfo>().GetHealth() >= 0)
                        {
                            isChasing = true;
                            isAttacking = false;
                        }

                        if (chasingObject.GetComponent<GlobalPlayerInfo>().GetHealth() <= 0)
                        {
                            backToDefault = true;
                            isChasing = false;
                            isAttacking = false;
                            chasingObject = null;
                        }
                    }
                } //is Attacting

                if (backToDefault)
                {
                    moveSpeed = (int) chasingSpeedMultiplier * moveSpeed;
                    ChangeFacingDirection( spawnPosition - gameObject.transform.position);
                    //Enemy gets full HP
                    gameObject.GetComponent<EnemyInfo>().BackToDefault();
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Patrolling", false);

                    if (Vector3.Distance(transform.position, spawnPosition) <= 1f)
                    {
                        movingDirection = RandomVector(-patrolRange, patrolRange);
                        backToDefault = false;
                        isGuarding = true;
                        chasingObject = null;
                        //Debug.Log("Back to guarding");
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, spawnPosition,
                            moveSpeed * 0.3f * Time.deltaTime);
                        //Change facing direction only once when moving back to default spot
                        //transform.LookAt(spawnPosition);
                    }
                }

                if ((!isChasing) && (!isAttacking))
                {
                    CheckForPlayer();
                }


                //calculate new movement based on obstacle his


                if (movingDirection != Vector3.zero && checkForHinder)
                {
                    Vector3 nevVector = CalculateMovement();
                    movingDirection = Vector3.Lerp(movingDirection, nevVector.normalized,
                        moveSpeed * Time.fixedDeltaTime);
                    waitFrame++;
                    if (waitFrame % 60 == 0)
                    {
                        ChangeFacingDirection(movingDirection);
                    }
                }

                //Foljande 2 rader skickar ett kommando till servern och da andrar antingen positionen eller rotationen samt HP
                CmdSetSynchedPosition(transform.position);
                CmdSetSynchedRotation(transform.rotation);
            } //is grounded
        } //is server
    }

    private void LateUpdate()
    {
        if (!isServer)
        {
            this.transform.position = syncPosition;
            this.transform.rotation = syncRotation;
        }

        waitFrame = 0;
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
                if (chasingObject == null &&
                    coll.gameObject.GetComponent<GlobalPlayerInfo>().GetHealth() >
                    0) //if we are not chasing someone, only aim one target
                {
                    chasingObject = coll.gameObject;
                    isGuarding = false;
                    isChasing = true;
                }
            }
        }
    }

    private Vector3 RandomVector(float min, float max) //version1 lock enemy's y-axel
    {
        Vector3 movingDirection =
            new Vector3(UnityEngine.Random.Range(min, max), 0.0f, UnityEngine.Random.Range(min, max));
        ChangeFacingDirection(movingDirection);
        return movingDirection;
    }

    private Vector3 RandomVector(Vector3 current)
    {
        float angle = UnityEngine.Random.Range(170f, 190f);
        Vector3 temp = Quaternion.Euler(0, angle, 0) * current;
        //ChangeFacingDirection(temp);
        return temp;
    }


    private void ChangeFacingDirection(Vector3 movingDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(movingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 1);
    }


    //Can be replaced if better solution found**
    private Vector3 CalculateMovement()
    {
        Vector3 movementVector = Vector3.zero;


        float stepAngle = (visionAngle * 2.0f) / (traces - 1);
        RaycastHit hitInfo1;
        if (Physics.Raycast(transform.position, movingDirection + transform.position, 3f, (1 << 9)))
        {
            //Time.timeScale = 0;

            movingDirection = -movingDirection;

            Debug.DrawLine(transform.position, transform.position + movingDirection, Color.green);
            return movingDirection;
        }
        //RaycastHit hitInfo;
        //if (Physics.CapsuleCast(point1, point2, capsCollider.radius, transform.position+movingDirection, out hitInfo, attackRange, (1 << 9)))
        //{
        //    if(hitInfo.collider.gameObject.tag == "House")
        //    {
        //        movingDirection = -movingDirection;
        //        return movingDirection;
        //    }

        //}
        // Create movement vector based on lidar sight.
        for (int i = 0; i < traces; i++)
        {
            float angle1 = (90.0f + visionAngle - (i * stepAngle)) * Mathf.Deg2Rad;
            Vector3 direction1 = transform.TransformDirection(new Vector3(Mathf.Cos(angle1), 0.0f, Mathf.Sin(angle1)));


            if (Physics.Raycast(transform.position, direction1, out hitInfo1, maxChasingRange + patrolRange,
                    ~(1 << 9 | 1 << 7)))
            {
                if (hitInfo1.collider.tag != "Player")
                {
                    movementVector += direction1 * (hitInfo1.distance - 3f);
                }
            }
            else
            {
                movementVector += direction1 * (maxChasingRange + patrolRange);
            }
        }

        return movementVector;
    }
    //****

    public void SetEnemyTransform(Transform tran)
    {
        spawnPosition = tran.position;
    }

    //Gets called from the item beviour
    public void Stagger()
    {
        hitsForStagger++;
        if (hitsForStagger >= 3)
        {
            hitsForStagger = 0;
            animator.SetBool("Staggered", true);
            animator.SetBool("Chasing", false);
            animator.SetBool("Attacking", false);
            animator.SetBool("Patrolling", false);
            StartCoroutine(WaitForStagger(0.933f));
        }
    }

    IEnumerator WaitForStagger(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Staggered", false);
    }
}