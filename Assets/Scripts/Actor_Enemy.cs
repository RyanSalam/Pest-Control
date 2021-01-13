using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor_Enemy : Actor
{
    #region Variables
    [Header("Show Debugs")]
    public bool isDebugging;

    [Header("Enemy Type")]
    [Tooltip("The type of enemy, used in spawning considerations.")]
    public EnemyType enemyType;

    [Header("Enemy Navigation Variables")]
    // Navigation agent, used to handle movement calculations.
    private NavMeshAgent _agent;
    public NavMeshAgent agent { get { return _agent; } }
    
    // Animator component, used to handle state machine and animations.
    private Animator _animator;
    public Animator animator { get { return _animator; } }

    // Target reference, used to switch the character's pathfinding destination.
    private Actor _target;
    public Actor target { get { return _target; } }

    // Boolean to track if the enemy is retaliating against an attacker.
    [HideInInspector] public bool isRetaliating;

    [Tooltip("Float determining the movement speed of the enemy.")]
    public float movementSpeed;

    [Tooltip("Float determining the amount of damage resisted when attacked outside the hit angle.")]
    public float armourStrength;

    [Tooltip("Float determining the amount of energy dropped upon the enemy's death.")]
    public float energyCarried;

    [Tooltip("Trigger box that activates upon the Enemy's attack.")]
    public BoxCollider attackBox;

    [Tooltip("Float determining the amount of damage the enemy will do on attack.")]
    public float attackDamage;

    [Tooltip("Float determining the cooldown time between successive attacks.")]
    public float rateOfFire;

    // Float determining the range the target has to be within for the Enemy to attack.
    private float _attackRange;
    public float attackRange { get { return _attackRange; } }

    [Tooltip("Layer that the players will be on for detection purposes.")]
    public LayerMask playerLayer;

    [Tooltip("Float determining the range the enemy can detect the player from, unless retaliating.")]
    public float detectionRadius;

    [Tooltip("Float determining the distance the player must be away from the enemy before the enemy loses aggro.")]
    public float detectionLossRange;

    [Tooltip("New target actor, for target switching purposes.")]
    public Actor newTarget;

    [Tooltip("Hard references for player and core to aid in target switching.")]
    public Actor player;
    public Actor core;
    #endregion

    protected override void Start()
    {
        base.Start();
        // Run the initialise function.
        Init();
    }

    protected override void Awake()
    {
        base.Awake();
        // Run the initialise function
        Init();
    }

    // Function to intialise variables that need initialised
    private void Init()
    {
        // Agent and Animator reference.
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        // Setting the movement speed of the Enemy's agent to the value of the Enemy's movement speed.
        _agent.speed = movementSpeed;

        // Setting the attack range to 3/4 the attack box.
        _attackRange = attackBox.bounds.extents.y + attackBox.bounds.extents.y / 2;

        //// Player and Core reference
        //player = LevelManager.Instance.player;
        //core = LevelManager.Instance.core;
    }

    private void OnDrawGizmosSelected()
    {
        if(isDebugging)
        {
            // Debug drawing a wire sphere to visualise the range a player has to be in in order to be detected.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Debug drawing a wire sphere to vialise the range a player has to exceed in order to be "lost" by the Enemy.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionLossRange);
        }
    }

    //Enumerator to define the Enemy Type, used by the Wave Manager to determine which enemies to spawn.
    public enum EnemyType
    {
        Grunt,
        Tank,
        Drone
    }
}
