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
    private GlobalPlayerInfo globalPlayerInfo;
    private bool isAttacking;
    private bool isStaggerd;
    private NavMeshAgent navMeshAgent;
    private float navMeshAgentSpeed;
    private GameObject spawnPoint;
    private int stateToPlayByIndex = 0;
    private GameObject target;
    private int hitsForStagger;
    private GameObject[] players;
    [SerializeField] private float aggroRangeFromSpawnPoint;
    [SerializeField] private bool canSeeThroughWalls;
    [SerializeField] private int hitAmountForStagger = 3;

    void Awake()
    {
        spawnPoint = new GameObject("EnemySpawnPoint");
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterBase = GetComponent<EnemyInfo>().GetCharacterBase();

        chasingSpeedMultiplier = characterBase.GetChasingSpeed();
        defaultSpeed = characterBase.GetMovementSpeed();
        damage = characterBase.GetDamage();
        attackRange = characterBase.GetRange();
        navMeshAgent.stoppingDistance = attackRange*0.8f;
        navMeshAgentSpeed = navMeshAgent.speed;
        
        target = spawnPoint;
    }

    void FixedUpdate()
    {
        if(isServer)
        {   
            if(stateToPlayByIndex == 1)
            {
                navMeshAgent.speed = navMeshAgentSpeed*chasingSpeedMultiplier;
            }else
            {
                navMeshAgent.speed = navMeshAgentSpeed;
            }
            animator.SetInteger("State",stateToPlayByIndex);
            if(stateToPlayByIndex != 4)
            {     
                SetTarget();
                
                if(!isAttacking && stateToPlayByIndex == 3)
                    StartCoroutine(Attack());
                
                if(currentDestination != target.transform.position)
                {
                    currentDestination = target.transform.position;
                    navMeshAgent.ResetPath();
                    navMeshAgent.SetDestination(target.transform.position);
                }
            }else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Staggered") && !isStaggerd)
            {
                StartCoroutine(StopStagger());
            }
                
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + new Vector3(0,1,0),target.transform.position + new Vector3(0,1,0));
    }

    private void SetTarget()
    {
        
        if(players != null)
        {
            foreach(GameObject player in players)
                if(Vector3.Distance(spawnPoint.transform.position,player.transform.position) <= aggroRangeFromSpawnPoint)
                {
                    target = player;
                    stateToPlayByIndex = 1;
                    
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
                    target = spawnPoint;
                    stateToPlayByIndex = 2; 
                }
                    
        }else
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        if (navMeshAgent.remainingDistance <= attackRange)
        {
            if(target.tag.Equals("Player"))
            {
                stateToPlayByIndex = 3;
                transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation((target.transform.position - transform.position).normalized),Time.deltaTime*navMeshAgent.angularSpeed);
            }
            else
            {
                stateToPlayByIndex = 5;
                StopAllCoroutines();
                isAttacking = false;
            }
                
        }                    
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (navMeshAgent.remainingDistance <= attackRange)
        {
            if(target.tag.Equals("Player"))
            {
                target.GetComponent<GlobalPlayerInfo>().UpdateHealth(-damage);

                // Creates an event used to play a sound and display the damage in the player UI
                EventInfo playerDamageEventInfo = new DamageEventInfo{ EventUnitGo = gameObject, target = this.target};
                EventSystem.Current.FireEvent(playerDamageEventInfo);
            }
        }
        isAttacking = false;
    }
    private IEnumerator StopStagger()
    {
        isStaggerd = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        stateToPlayByIndex = 5;
        isStaggerd = false;
    }

    public void SetEnemyTransform(Transform trans)
    {
        spawnPoint.transform.position = trans.position;
    }

    public void Stagger()
    {
        hitsForStagger++;
        if(hitsForStagger >= hitAmountForStagger)
        {
            hitsForStagger = 0;
            StopAllCoroutines();
            navMeshAgent.ResetPath();
            stateToPlayByIndex = 4;
            isAttacking = false;
        }
        
    }
}
