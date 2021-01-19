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


    [Header("Behaviour Variables")]
    [Tooltip("Float determining the amount of damage resisted when attacked outside the hit angle.")]
    public float armourStrength;

    [Tooltip("Float determining the amount of energy dropped upon the enemy's death.")]
    public float energyCarried;

    [Tooltip("Trigger box that activates upon the Enemy's attack.")]
    public GameObject attackBox;

    [Tooltip("Float determining the amount of damage the enemy will do on attack.")]
    public float attackDamage;

    [Tooltip("Float determining the cooldown time between successive attacks.")]
    public float rateOfFire;

    [Tooltip("Float determining the range the target has to be within for the Enemy to attack.")]
    public float attackRange;

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
    public Actor_Core core;

    // Int to track how many times the enemy has been hit.
    [HideInInspector] public int hitsRecieved = 0;
    #endregion

    protected override void Start()
    {
        base.Start();
        // Run the initialise function.
        
        // Agent and Animator reference.
        _agent = GetComponent<NavMeshAgent>();

        // Set the agent stopping distance to half the size of the attack box.
        _agent.stoppingDistance = attackRange;

        // Setting the movement speed of the Enemy's agent to the value of the Enemy's movement speed.
        _agent.speed = moveSpeed;

        // Player and Core reference
        player = LevelManager.Instance.Player;
        core = LevelManager.Instance.Core;
    }

    protected override void Awake()
    {
        base.Awake();
        // Run the initialise function
    }

    private void Update()
    {
        // If not retaliating and target and new target differ set target to new target.
        if(!isRetaliating && target != newTarget)
            _target = newTarget;

        // If stopped and there is a target then trigger the attack animation.
        if (target != null && Vector3.Distance(target.transform.position, attackBox.transform.position) <= attackRange)
            Anim.SetTrigger("Attacking");

        // If the target is the player and they are outside detection loss range switch to core, else default to core
        if (target == player && Vector3.Distance(target.transform.position, transform.position) > detectionLossRange)
            newTarget = core;
        else
            newTarget = core;

        // Set the target on the animator accordingly.
        if (target)
            Anim.SetBool("hasTarget", true);
        else
            Anim.SetBool("hasTarget", false);
    }

    // On taking damage, increase the hits recieved and if enemy is a grunt it will persue and attack their attacker.
    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        hitsRecieved++;
        Anim.SetInteger("hitsRecieved", hitsRecieved);

        if(enemyType == EnemyType.Grunt)
        {
            newTarget = data.damager;
            isRetaliating = true;
        }
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

            // Debug drawing a wire cube to visualise the attack box of the Enemy.
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(attackBox.transform.position, Vector3.one * attackRange);
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
