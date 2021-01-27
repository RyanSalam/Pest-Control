using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : Actor
{
    // Components

    [SerializeField] protected NavMeshAgent m_agent;

    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected float damage = 1.0f;
    [SerializeField] protected bool bIsSearching = false;

    [SerializeField] protected int energyDrop = 10;

    #region Getters

    public NavMeshAgent Agent
    {
        get { return m_agent; }
    }

    #endregion

    [SerializeField] protected Transform currentTarget;
    [SerializeField] protected Vector3 currentDestination;

    public Transform CurrentTarget 
    {
        get { return currentTarget; }
    }
    public Vector3 CurrentDestination
    {
        get { return currentDestination; }
    }


    private Actor_Player m_player;
    public Actor_Player Player
    {
        get { return m_player; }
    }

    private Actor_Core m_core;
    public Actor_Core Core
    {
        get { return m_core; }
    }

    protected DamageData _lastCachedDamage;
    public DamageData LastCachedDamage
    {
        get { return _lastCachedDamage; }
    }

    protected NavMeshPath _currentPath;
    public NavMeshPath CurrentPath
    {
        get { return _currentPath; }
    }

    protected override void Start()
    {
        base.Start();

        m_player = LevelManager.Instance.Player;
        m_core = LevelManager.Instance.Core;

        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = true;

        OnDeath += () => LevelManager.Instance.CurrentEnergy += energyDrop;
    }

    // Function 
    public abstract void OnPathCompleted();

    public void SwitchTarget(Transform newTarget)
    {
        if (currentTarget == newTarget) return;

        currentTarget = newTarget;
        currentDestination = newTarget.position;
    }

    public bool GetRandomPointAroundTarget(Vector3 target, float range, out Vector3 result)
    {
        for(int i = 0; i < 30; i++)
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
