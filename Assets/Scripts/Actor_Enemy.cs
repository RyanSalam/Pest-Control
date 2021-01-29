using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actor_Enemy : Actor
{
    //#region Variables
    //[Header("Enemy Type")]
    //[Tooltip("The type of enemy, used in spawning considerations.")]
    //public EnemyType enemyType;

    //[Header("Enemy Navigation Variables")]
    //// Navigation agent, used to handle movement calculations.
    //private NavMeshAgent _agent;
    //public NavMeshAgent agent { get { return _agent; } }

    //// Animator component, used to handle state machine and animations.
    //private Animator _animator;
    //public Animator animator { get { return _animator; } }

    //// Target reference, used to switch the character's pathfinding destination.
    //private Actor _target;
    //public Actor target { get { return _target; } }

    //// Boolean to track if the enemy is retaliating against an attacker.
    //[HideInInspector] public bool isRetaliating;


    //[Header("Behaviour Variables")]
    //[Tooltip("Float determining the amount of damage resisted when attacked outside the hit angle.")]
    //public float armourStrength;

    //[Tooltip("Float determining the amount of energy dropped upon the enemy's death.")]
    //public float energyCarried;

    //[Tooltip("Trigger box that activates upon the Enemy's attack.")]
    //public GameObject attackBox;

    //[Tooltip("Float determining the amount of damage the enemy will do on attack.")]
    //public float attackDamage;

    //[Tooltip("Float determining the cooldown time between successive attacks.")]
    //public float rateOfFire;

    //[Tooltip("Float determining the range the target has to be within for the Enemy to attack.")]
    //public float attackRange;

    //[Tooltip("Layer that the players will be on for detection purposes.")]
    //public LayerMask playerLayer;

    //[Tooltip("Float determining the range the enemy can detect the player from, unless retaliating.")]
    //public float detectionRadius;

    //[Tooltip("Float determining the distance the player must be away from the enemy before the enemy loses aggro.")]
    //public float detectionLossRange;

    //[Tooltip("Hard references for player and core to aid in target switching.")]
    //public Actor player;
    //public Actor_Core core;

    //// Int to track how many times the enemy has been hit.
    //[HideInInspector] public int hitsRecieved = 0;
    //#endregion

    //protected override void Start()
    //{
    //    base.Start();
    //    // Run the initialise function.

    //    // Agent and Animator reference.
    //    _agent = GetComponent<NavMeshAgent>();

    //    // Set the agent stopping distance to half the size of the attack box.
    //    _agent.stoppingDistance = attackRange;

    //    // Setting the movement speed of the Enemy's agent to the value of the Enemy's movement speed.
    //    _agent.speed = moveSpeed;

    //    // Player and Core reference
    //    player = LevelManager.Instance.Player;
    //    core = LevelManager.Instance.Core;
    //}

    //protected override void Awake()
    //{
    //    base.Awake();
    //    // Run the initialise function
    //}

    //private void Update()
    //{ 
    //    // Set the target on the animator accordingly.
    //    if (target)
    //        Anim.SetBool("hasTarget", true);
    //    else
    //        Anim.SetBool("hasTarget", false);
    //}

    //public void Search()
    //{
    //    // Create an overlap sphere to search the area around the enemy for any colliders on the player layer.
    //    Collider[] targets = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

    //    Debug.Log("Searching");

    //    // If any targets are found set newTarget to the first found player.
    //    if (targets.Length > 0 && Vector3.Distance(core.transform.position, transform.position) > detectionRadius)
    //    {
    //        SwitchTarget(player);
    //        Debug.Log("Targting Player");
    //    }
    //    else
    //    {
    //        SwitchTarget(core);
    //        Debug.Log("Targeting Core");
    //    }
    //}    

    //public void SwitchTarget(Actor newTarget)
    //{
    //    if(_target != newTarget)
    //    {
    //        _target = newTarget;
    //    }

    //    EnemyManager.Instance.ReassesTargets(null);
    //}

    //// On taking damage, increase the hits recieved and if enemy is a grunt it will persue and attack their attacker.
    //public override void TakeDamage(DamageData data)
    //{
    //    base.TakeDamage(data);
    //    hitsRecieved++;
    //    Anim.SetInteger("hitsRecieved", hitsRecieved);

    //    if(enemyType == EnemyType.Grunt)
    //    {
    //        SwitchTarget(data.damager);
    //        isRetaliating = true;
    //    }
    //}

    //protected override void Death()
    //{
    //    base.Death();

    //    Destroy(gameObject);
    //}

    ////Enumerator to define the Enemy Type, used by the Wave Manager to determine which enemies to spawn.
    //public enum EnemyType
    //{
    //    Grunt,
    //    Tank,
    //    Drone
    //}

    #region Variables
    // Navigation variables
    [SerializeField] protected Transform currentTarget;
    [SerializeField] protected Vector3 currentDestination;

    // Agent reference for navigation
    protected NavMeshAgent m_agent;

    // Player and core reference for easy target switching
    private Actor_Player m_player;
    private Actor_Core m_core;

    // Damage data reference to track last damage instance
    protected DamageData _lastCachedDamage;

    // Nav mesh path to track current path for vector manipulation
    protected NavMeshPath _currentPath;

    // Behviour variables
    [SerializeField] protected float _attackRange = 2.5f;
    [SerializeField] protected float _damage = 1.0f;
    [SerializeField] protected bool _bIsSearching = false;
    [SerializeField] protected LayerMask _playerLayer;

    // Interger defining how much energy enemy will drop upon death
    [SerializeField] protected int _energyDrop = 10;

    #region Getters
    // Agent getter for out of class access
    public NavMeshAgent Agent
    {
        get { return m_agent; }
    }

    // Current target getter for out of class access
    public Transform CurrentTarget
    {
        get { return currentTarget; }
    }

    // Current destination getter for out of class access
    public Vector3 CurrentDestination
    {
        get { return currentDestination; }
    }

    // Player getter for out of class access
    public Actor_Player Player
    {
        get { return m_player; }
    }

    // Core getter for out of class access
    public Actor_Core Core
    {
        get { return m_core; }
    }

    // Last cached damage getter for out of class access
    public DamageData LastCachedDamage
    {
        get { return _lastCachedDamage; }
    }

    // Current path getter for out of class access
    public NavMeshPath CurrentPath
    {
        get { return _currentPath; }
    }

    // Damage getter for out of class access
    public float Damage
    {
        get { return _damage; }
    }

    // Attack Range getter for out of class access
    public float AttackRange
    {
        get { return _attackRange; }
    }

    // Player Layer getter for out of class access
    public float PlayerLayer
    {
        get { return _attackRange; }
    }
    #endregion
    #endregion

    // Start to initialise variables
    protected override void Start()
    {
        base.Start();

        m_player = LevelManager.Instance.Player;
        m_core = LevelManager.Instance.Core;

        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = true;

        OnDeath += () => LevelManager.Instance.CurrentEnergy += _energyDrop;
    }

    protected virtual void LateUpdate()
    {
        // Setting the animator booleans according to their corresponding conditions
        Anim.SetBool("hasArrived", Agent.pathStatus == NavMeshPathStatus.PathComplete);
        Anim.SetBool("hasTarget", currentTarget != null);
    }

    // Function to define a behaviour that will run upon path completion
    public abstract void OnPathCompleted();

    // Function to change target to the passed new target & update the pathfinding
    public void SwitchTarget(Transform newTarget)
    {
        if (currentTarget == newTarget) return;

        currentTarget = newTarget;
        currentDestination = newTarget.position;
    }

    // Function to randomise a position around the target to better vary pathfinding between enemies
    public bool GetRandomPointAroundTarget(Vector3 target, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randPoint = target + Random.insideUnitSphere * range;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    // Function to set the destination according to the random point function
    public void SetDestinationAroundTarget(Vector3 targetPos, float range)
    {
        Vector3 result;

        if (GetRandomPointAroundTarget(targetPos, range, out result))
        {
            m_agent.SetDestination(result);
            return;
        }
    }
}
