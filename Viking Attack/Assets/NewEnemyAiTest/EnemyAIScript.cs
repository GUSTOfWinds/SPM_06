using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;
using Mirror;
using UnityEngine.AI;
using Event;

public class EnemyAIScript : NetworkBehaviour
{
    private Animator animator;
    private float attackRange;
    private CharacterBase characterBase;
    private float chasingSpeedMultiplier;
    private bool chasing;
    private Vector3 currentDestination;
    private float damage;
    private float defaultSpeed;
    private bool isAttacking;
    private bool isStaggerd;
    private NavMeshAgent navMeshAgent;
    private bool roaming;
    private GameObject roamingPoint;
    private GameObject spawnPoint;
    private int stateToPlayByIndex = 0;
    private GameObject target;
    private int hitsForStagger;
    private GameObject[] players;
    [SerializeField] private float aggroRangeFromSpawnPoint;
    [SerializeField] private bool canSeeThroughWalls;
    [SerializeField] private int hitAmountForStagger = 3;
    [SerializeField] private float roamingRangeFromSpawn;

    // Syncs the position of the object to the server
    [SyncVar] private Vector3 syncPosition;

    // Syncs the rotaion of the object to the server
    [SyncVar] private Quaternion syncRotation;

    void Awake()
    {
        //Creates object that enemy uses as points to move to
        spawnPoint = new GameObject("EnemySpawnPoint");
        roamingPoint = new GameObject("RoamingPoint");
    }

    void Start()
    {
        //Gets components that are on the enemy object that we need to be able to reference 
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterBase = GetComponent<EnemyInfo>().GetCharacterBase();

        //Gets the variables that we need to set from the characterBase
        chasingSpeedMultiplier = characterBase.GetChasingSpeed();
        defaultSpeed = characterBase.GetMovementSpeed();
        damage = characterBase.GetDamage();
        attackRange = characterBase.GetRange();
        navMeshAgent.speed = characterBase.GetMovementSpeed();
        
        //Sets that the enemy stop a bit closer then there hit range
        navMeshAgent.stoppingDistance = attackRange*0.8f;
        
        //Sets starting target
        target = spawnPoint;
    }

    void FixedUpdate()
    {
        if(isServer)
        {   
            //Is the enemy running set speed to chasingSpeedMultiplier fast if not set speed to defaultSpeed
            if(stateToPlayByIndex == 1)
            {
                navMeshAgent.speed = defaultSpeed*chasingSpeedMultiplier;
            }else
            {
                navMeshAgent.speed = defaultSpeed;
            }

            //Tells the animator what animation to play
            animator.SetInteger("State",stateToPlayByIndex);

            //If the enemy is staggered don't check for new path
            if(stateToPlayByIndex != 4)
            {     
                //Find what target the enemy should follow
                SetTarget();
                
                //Check if the enemy should attack and that Attack() function is not going if both true start timer for attack
                if(!isAttacking && stateToPlayByIndex == 3)
                {
                    StartCoroutine(Attack());
                }
                    
                //Checks if the target is in the same position, if not delete current path and find new path
                if(currentDestination != target.transform.position)
                {
                    currentDestination = target.transform.position;
                    navMeshAgent.ResetPath();
                    navMeshAgent.SetDestination(target.transform.position);
                }

            //Check if the Staggered animation is playing and that StopStagger() function is not going if both true start timer for stagger
            }else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Staggered") && !isStaggerd)
            {
                StartCoroutine(StopStagger());
            }
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
    [Command(requiresAuthority = false)] void CmdSetSynchedPosition(Vector3 position) => syncPosition = position;

    [Command(requiresAuthority = false)] void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;

    void OnDrawGizmos()
    {
        //Shows where the enemy is going (not the path)
        Gizmos.DrawLine(transform.position + new Vector3(0,1,0),target.transform.position + new Vector3(0,1,0));
    }

    private void SetTarget()
    {
        //Would want to remove, maybe check when a new player joins?
        players = GameObject.FindGameObjectsWithTag("Player");
        //
        //Checks if there are any palyers the the players list
        if(players != null)
        {
            //For each player in players list check if that player is in agro range of enemy if not set target to roamingPoint or spawnPoint 
            foreach(GameObject player in players)
                if(Vector3.Distance(spawnPoint.transform.position,player.transform.position) <= aggroRangeFromSpawnPoint)
                {
                    target = player;
                    stateToPlayByIndex = 1;
                    
                    //Checks if there are anything between the enemy and player, if not don't check until enemy loses aggro
                    RaycastHit hit;
                    if(!canSeeThroughWalls && !chasing && Physics.Linecast(transform.position + new Vector3(0,1,0),target.transform.position + new Vector3(0,1,0),out hit,~LayerMask.GetMask("Player","Enemy")))
                    {
                        target = spawnPoint;
                        stateToPlayByIndex = 2; 
                    }else
                    {
                        chasing = true;
                    }
                    
                }else
                {
                    chasing = false;
                    stateToPlayByIndex = 2; 

                    if(roaming)
                        target = roamingPoint;
                    else
                        target = spawnPoint;
                    
                }
        //Checks for GameObjects with Player tag  
        }else
            players = GameObject.FindGameObjectsWithTag("Player");
        
        //Checks if the enemy is att there target
        if (navMeshAgent.remainingDistance <= attackRange)
        {
            //Checks if target is a player if true set that the enemy should attack and rotate towards the player
            if(target.tag.Equals("Player"))
            {
                stateToPlayByIndex = 3;
                transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation((target.transform.position - transform.position).normalized),Time.deltaTime*navMeshAgent.angularSpeed);
            //If target is a spawnPoint set that the enemy should idle
            }else if(target == spawnPoint)
            {
                stateToPlayByIndex = 5;
            //If target is a roamingPoint set new position for roamingPoint
            }else
            {
                Vector3 randomDirection = Random.insideUnitSphere * roamingRangeFromSpawn;
                randomDirection += new Vector3(spawnPoint.transform.position.x,transform.position.y,spawnPoint.transform.position.z);
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, roamingRangeFromSpawn, 1);
                roamingPoint.transform.position = hit.position;
            }
        }else
        {
            StopAllCoroutines();
        }            
    }

    private IEnumerator Attack()
    {
        //Sets isAttacking to true to show that the Attack() function is running
        isAttacking = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        //Checks if the player is still in range
        if (navMeshAgent.remainingDistance <= attackRange)
            if(target.tag.Equals("Player"))
            {
                target.GetComponent<GlobalPlayerInfo>().UpdateHealth(-damage);

                // Creates an event used to play a sound and display the damage in the player UI
                EventInfo playerDamageEventInfo = new DamageEventInfo{ EventUnitGo = gameObject, target = this.target};
                EventSystem.Current.FireEvent(playerDamageEventInfo);
            }
        //Sets isAttacking to false to show that the Attack() function is done
        isAttacking = false;
    }
    private IEnumerator StopStagger()
    {
        //Sets isStaggerd to true to show that the StopStagger() function is running
        isStaggerd = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        stateToPlayByIndex = 5;
        //Sets isStaggerd to false to show that the StopStagger() function is done
        isStaggerd = false;
    }
    public void Stagger()
    {
        //Counts hits until hitAmountForStagger the stagger enemy which also resets path that stop the enemy form moving
        hitsForStagger++;
        if(hitsForStagger >= hitAmountForStagger)
        {
            hitsForStagger = 0;
            navMeshAgent.ResetPath();
            stateToPlayByIndex = 4;
            //Stops the Attack() function
            StopAllCoroutines();
            //Sets isAttacking to false to show that the Attack() function is done
            isAttacking = false;
        }
        
    }
    public void SetEnemyTransform(Transform trans)
    {
        spawnPoint.transform.position = trans.position;
    }
    public void SetIfEnemyRoam(bool roaming)
    {
        this.roaming = roaming;
    }
}
