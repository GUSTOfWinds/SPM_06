using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;
using Mirror;
using UnityEngine.AI;

public class EnemyScript : NetworkBehaviour
{
    private GameObject target;
    private Animator animator;
    private CharacterBase characterBase;
    private float chasingSpeedMultiplier;
    private Vector3 currentDestination;
    private float defaultSpeed;
    private NavMeshAgent navMeshAgent;
    private GameObject spawnPoint;
    private int stateToPlayByIndex = 0;
    private GameObject[] players;
    [SerializeField] private float aggroRangeFromSpawnPoint;

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
        navMeshAgent.stoppingDistance = characterBase.GetRange();
        
        target = spawnPoint;
    }

    void FixedUpdate()
    {
        if(isServer)
        {        
            SetTarget();
            animator.SetInteger("State",stateToPlayByIndex);
            
            if(currentDestination != target.transform.position)
            {
                currentDestination = target.transform.position;
                navMeshAgent.ResetPath();
                navMeshAgent.SetDestination(target.transform.position);
            }
        }
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
                }else
                {
                    target = spawnPoint;
                    stateToPlayByIndex = 2; 
                }
                    
        }else
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if(target.tag.Equals("Player"))
                stateToPlayByIndex = 3;
            else
                stateToPlayByIndex = 5;
        }                    
    }

    public void SetEnemyTransform(Transform trans)
    {
        spawnPoint.transform.position = trans.position;
    }

    public Animator Animator => animator;
    public float StateToPlayByIndex => stateToPlayByIndex;
    public GameObject Target => target;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
}
