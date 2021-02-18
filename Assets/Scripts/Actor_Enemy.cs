using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actor_Enemy : Actor
{
    #region Variables
    // Navigation variables
    [SerializeField] protected Transform currentTarget;
    [SerializeField] protected Vector3 currentDestination;

    // Agent reference for navigation
    protected NavMeshAgent m_agent;

    // Player and core reference for easy target switching
    public Actor_Player m_player;
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

    protected Timer _intervalTimer;
    [SerializeField] protected float scanIntervals = 2.0f;

    public float movementSpeed = 3.5f;

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

    public Timer IntervalTimer
    {
        get { return _intervalTimer; }
    }
    #endregion
    #endregion

    protected override void Awake()
    {
        base.Awake();

        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = true;
        m_agent.speed = movementSpeed;

        OnDeath += () => LevelManager.Instance.CurrentEnergy += _energyDrop;

        _intervalTimer = new Timer(scanIntervals);
        _intervalTimer.OnTimerEnd += OnBtwnIntervals;
    }

    // Start to initialise variables
    protected override void Start()
    {
        base.Start();

        m_player = LevelManager.Instance.Player;
        m_core = LevelManager.Instance.Core;

        
    }

    protected virtual void Update()
    {
        _intervalTimer.Tick(Time.deltaTime);
    }

    protected virtual void LateUpdate()
    {
        // Setting the animator booleans according to their corresponding conditions
        Anim.SetBool("hasArrived", Vector3.Distance(transform.position, currentTarget.position) <= _attackRange);
        Anim.SetBool("hasTarget", currentTarget != Core.transform);
    }

    // Function that gets called each time the timer has finished ticking. 
    // Could be used for various reasons such as redirecting path 
    // scanning for player
    public virtual void OnBtwnIntervals() { }

    // Function to define a behaviour that will run upon path completion
    public abstract void OnPathCompleted();

    // Function to change target to the passed new target & update the pathfinding
    public void SwitchTarget(Transform newTarget)
    {
        if (currentTarget == newTarget) return;

        currentTarget = newTarget;
        currentDestination = newTarget.position;

        if (GetRandomPointAroundTarget(currentTarget.position, _attackRange, out currentDestination))
        {
            Agent.SetDestination(currentDestination);
        }
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
