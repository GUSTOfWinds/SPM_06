using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;
using Mirror;
using UnityEngine.AI;
using Event;
using Random = UnityEngine.Random;

public class EnemyAIScript : NetworkBehaviour
{
    /*
        @Author Love Strignert - lost9373
    */
    private Animator animator;
    private float attackRange;
    private AudioSource audioSource;
    private CharacterBase characterBase;
    private float chasingSpeedMultiplier;
    private bool chasing;
    private Vector3 currentDestination;
    private float damage;
    private float defaultSpeed;
    private DeathListener deathListener;
    private EnemyVitalController enemyVitalController;
    private bool isAttacking;
    private bool isStaggerd;
    private NavMeshAgent navMeshAgent;
    private Guid playerConnectGuid;
    private bool roaming;
    private GameObject roamingPoint;
    private GameObject spawnPoint;
    private int staggerStamina;
    private int stateToPlayByIndex = 0;
    private GameObject target;
    private GameObject[] players;
    private GameObject[] enemies;

    [SerializeField] private float aggroRangeFromSpawnPoint;
    [SerializeField] private bool canSeeThroughWalls;
    [SerializeField] private AudioClip[] enemySounds;
    [SerializeField] private float roamingRangeFromSpawn;

    // Syncs the position of the object to the server
    [SyncVar] private Vector3 syncPosition;
    // Syncs the rotaion of the object to the server
    [SyncVar] private Quaternion syncRotation;
    // Syncs the hitsForStagger of the object to the server
    [SyncVar] private int syncHitsForStagger;

    void Awake()
    {
        //Creates object that enemy uses as points to move to
        spawnPoint = new GameObject("EnemySpawnPoint");
        roamingPoint = new GameObject("RoamingPoint");
        EventSystem.Current.RegisterListener<PlayerConnectEventInfo>(UpdatePlayerList, ref playerConnectGuid);
    }

    void Start()
    {
        //Gets components that are on the enemy object that we need to be able to reference 
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterBase = GetComponent<EnemyInfo>().GetCharacterBase();
        enemyVitalController = GetComponent<EnemyVitalController>();

        //Gets the variables that we need to set from the characterBase
        chasingSpeedMultiplier = characterBase.GetChasingSpeed();
        defaultSpeed = characterBase.GetMovementSpeed();
        damage = characterBase.GetDamage();
        attackRange = characterBase.GetRange();
        navMeshAgent.speed = characterBase.GetMovementSpeed();
        staggerStamina = characterBase.GetStaggerStamina();

        //Sets that the enemy stop a bit closer then there hit range
        navMeshAgent.stoppingDistance = attackRange * 0.8f;

        //Sets starting target
        target = spawnPoint;
        roamingPoint.transform.position = gameObject.transform.position;

        if (isServer)
        {
            deathListener = FindObjectOfType<DeathListener>();
        }
    }


    void FixedUpdate()
    {
        if (isServer)
        {
            if (syncHitsForStagger >= staggerStamina)
            {
                Debug.Log("HMMMMMMMMMMMMMMMMM");
                navMeshAgent.velocity = Vector3.zero;
                CmdSetSynchedHitsForStagger(0);
                stateToPlayByIndex = 4;
                //Stops the Attack() function
                StopAllCoroutines();
                //Sets isAttacking to false to show that the Attack() function is done
                isAttacking = false;
            }

            //Is the enemy running set speed to chasingSpeedMultiplier fast if not set speed to defaultSpeed
            if (stateToPlayByIndex == 1)
            {
                navMeshAgent.speed = defaultSpeed * chasingSpeedMultiplier;
            }
            else
            {
                navMeshAgent.speed = defaultSpeed;
            }

            //Tells the animator what animation to play
            animator.SetInteger("State", stateToPlayByIndex);

            //If the enemy is staggered don't check for new path
            if (stateToPlayByIndex != 4)
            {
                //Find what target the enemy should follow
                SetTarget();

                //Check if the enemy should attack and that Attack() function is not going if both true start timer for attack
                if (!isAttacking && stateToPlayByIndex == 3 && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    StartCoroutine(Attack());
                }

                //Checks if the target is in the same position, if not delete current path and find new path
                if (currentDestination != target.transform.position)
                {
                    currentDestination = target.transform.position;
                    navMeshAgent.ResetPath();
                    navMeshAgent.SetDestination(target.transform.position);
                }

                //Check if the Staggered animation is playing and that StopStagger() function is not going if both true start timer for stagger
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Staggered") && !isStaggerd)
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

    private void UpdatePlayerList(PlayerConnectEventInfo playerConnectEventInfo)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void SetTarget()
    {
        //Would want to remove, maybe check when a new player joins?
        players = GameObject.FindGameObjectsWithTag("Player");
        //
        //Checks if there are any palyers the the players list
        if (players != null)
        {
            //For each player in players list check if that player is in agro range of enemy if not set target to roamingPoint or spawnPoint
            bool playerFound = false;
            List<GameObject> playersInRange = new List<GameObject>();
            foreach (GameObject player in players)
                if (Vector3.Distance(spawnPoint.transform.position, player.transform.position) <=
                    aggroRangeFromSpawnPoint)
                    playersInRange.Add(player);

            if (playersInRange.Count > 1 && enemyVitalController.aggroCounter.Count > 0)
            {
                GameObject tempPlayer = target;
                float tempAggro = 0;
                foreach (GameObject player in playersInRange)
                {
                    if (enemyVitalController.aggroCounter.ContainsKey(player.GetComponent<NetworkIdentity>().netId))
                        if (enemyVitalController.aggroCounter[player.GetComponent<NetworkIdentity>().netId] < tempAggro)
                        {
                            tempAggro = enemyVitalController.aggroCounter[player.GetComponent<NetworkIdentity>().netId];
                            tempPlayer = player;
                        }
                }

                target = tempPlayer;
                playerFound = true;
                stateToPlayByIndex = 1;
            }
            else if (playersInRange.Count >= 1)
            {
                GameObject player = playersInRange[0];
                playerFound = true;
                target = player;
                stateToPlayByIndex = 1;


                //Checks if there are anything between the enemy and player, if not don't check until enemy loses aggro
                RaycastHit hit;
                if (!canSeeThroughWalls && !chasing && Physics.Linecast(transform.position + new Vector3(0, 1, 0),
                        target.transform.position + new Vector3(0, 1, 0), out hit,
                        ~LayerMask.GetMask("Player", "Enemy")))
                {
                    target = spawnPoint;
                    stateToPlayByIndex = 2;
                    playerFound = false;
                }
                else
                {
                    chasing = true;
                }
            }
            else
            {
                enemyVitalController.aggroCounter.Clear();
            }

            if (!playerFound)
            {
                enemyVitalController.UpdateHealth(characterBase.GetMaxHealth());

                if (chasing)
                {
                    EventInfo enemyRetreatingEventInfo = new EnemyRetreatingEventInfo
                    {
                        EventUnitGo = gameObject,
                        netid = gameObject.GetComponent<NetworkIdentity>().netId
                    };
                    EventSystem.Current.FireEvent(enemyRetreatingEventInfo);
                }

                chasing = false;
                stateToPlayByIndex = 2;

                if (roaming)
                    target = roamingPoint;
                else
                    target = spawnPoint;
            }
            //Checks for GameObjects with Player tag  
        }
        else
            players = GameObject.FindGameObjectsWithTag("Player");

        //Checks if the enemy is att there target
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= attackRange)
        {
            //Checks if target is a player if true set that the enemy should attack and rotate towards the player
            if (target.tag.Equals("Player"))
            {
                stateToPlayByIndex = 3;
                if (!GetNearbyAudioSourcePlaying() && !audioSource.isPlaying)
                {
                    // plays the sound of the skeleton breathing when in range for attack
                    audioSource.PlayOneShot(enemySounds[0]);
                    RpcPlayEnemyChasing();
                }

                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation((target.transform.position - transform.position).normalized),
                    Time.deltaTime * navMeshAgent.angularSpeed);
                //If target is a spawnPoint set that the enemy should idle
            }
            else if (target == spawnPoint)
            {
                stateToPlayByIndex = 5;
                //If target is a roamingPoint set new position for roamingPoint
            }
            else
            {
                Vector3 randomDirection = Random.insideUnitSphere * roamingRangeFromSpawn;
                randomDirection += new Vector3(spawnPoint.transform.position.x, transform.position.y,
                    spawnPoint.transform.position.z);
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, roamingRangeFromSpawn, 1);
                roamingPoint.transform.position = hit.position;
            }
        }
        else
        {
            StopAllCoroutines();
            isAttacking = false;
        }
    }

    private bool GetNearbyAudioSourcePlaying()
    {
        enemies = deathListener.GetEnemies();
        foreach (var enemy in enemies)
            if (enemy != null)
                if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) < 6f &&
                    enemy.GetComponent<AudioSource>().isPlaying && !enemy.Equals(gameObject))
                    return true;
        return false;
    }

    [ClientRpc]
    private void RpcPlayEnemyChasing()
    {
        if (!isServer)
            audioSource.PlayOneShot(enemySounds[0]);
    }

    [ClientRpc]
    private void RpcSwingSword()
    {
        if (!isServer)
            audioSource.PlayOneShot(enemySounds[1]);
    }

    [ClientRpc]
    private void RpcDealDamage(GameObject player)
    {
        if (!isServer)
            player.GetComponent<GlobalPlayerInfo>().UpdateHealth(-damage);
    }

    private IEnumerator Attack()
    {
        navMeshAgent.velocity = Vector3.zero;
        //Sets isAttacking to true to show that the Attack() function is running
        isAttacking = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 0.5f);

        //Play attack sound for host and clients
        RpcSwingSword();
        audioSource.PlayOneShot(enemySounds[1]);

        //Checks if the player is still in range
        if (navMeshAgent.remainingDistance <= attackRange)
            if (target.tag.Equals("Player"))
            {
                RpcDealDamage(target);
                target.GetComponent<GlobalPlayerInfo>().UpdateHealth(-damage);

                // Creates an event used to play a sound and display the damage in the player UI
                EventInfo playerDamageEventInfo = new PlayerDamageEventInfo
                    {EventUnitGo = gameObject, target = this.target};
                EventSystem.Current.FireEvent(playerDamageEventInfo);
            }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 0.5f);
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

    public void Stagger(int amount)
    {
        //Counts hits for the stagger
        CmdSetSynchedHitsForStagger(syncHitsForStagger + amount);
        Debug.Log(syncHitsForStagger);
    }

    [ClientRpc]
    public void RpcBeforeDying(GameObject spawnPoint, GameObject roamingPoint)
    {
        if (!isServer)
        {
            Destroy(spawnPoint);
            Destroy(roamingPoint);
        }
    }

    public GameObject GetSpawnPoint => spawnPoint;
    public GameObject GetRoamingPoint => roamingPoint;
    public void SetIfEnemyRoam(bool roaming) => this.roaming = roaming;
    public void SetEnemyTransform(Transform trans) => spawnPoint.transform.position = trans.position;
    public void SetAggroRange(float aggroRange) => this.aggroRangeFromSpawnPoint = aggroRange;
    public void SetRoamingRange(float roamingRange) => this.roamingRangeFromSpawn = roamingRange;
    [Command(requiresAuthority = false)] private void CmdSetSynchedPosition(Vector3 position) => syncPosition = position;
    [Command(requiresAuthority = false)] private void CmdSetSynchedRotation(Quaternion rotation) => syncRotation = rotation;
    [Command(requiresAuthority = false)] private void CmdSetSynchedHitsForStagger(int amount) => syncHitsForStagger = amount;
    
}