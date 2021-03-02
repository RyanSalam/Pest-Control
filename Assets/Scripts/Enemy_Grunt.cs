using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grunt : Actor_Enemy
{
    Scanner<Actor_Player> playerScanner;
    public bool HiveDictated;

    [Range(0, 360)] public float detectionAngle;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDestinationAroundTarget(CurrentDestination, AttackRange);
        if(WaveManager.Instance.isBuildPhase == false)
            EnemyHiveMind.Instance.RegisterGrunt(this);
    }

    protected override void Start()
    {
        base.Start();

        // Initialising the player scanner properly passing relevant variables
        playerScanner = new Scanner<Actor_Player>(transform);
        playerScanner.targetMask = _playerLayer;
        playerScanner.detectionRadius = AttackRange * 2;
        playerScanner.detectionAngle = detectionAngle;

        if (GetRandomPointAroundTarget(currentTarget.position, _attackRange, out currentDestination))
        {
            Agent.SetDestination(currentDestination);
        }

        Agent.autoTraverseOffMeshLink = false;
    }

    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, currentTarget.position) <= _attackRange * 1.5f)
        {
            Vector3 dir = currentTarget.position - transform.position;
            dir.y = 0;

            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.6f);
        }

        else
        {
            Quaternion rot = Quaternion.LookRotation(Agent.velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.6f);
        }
    }

    public override void OnBtwnIntervals()
    {
        base.OnBtwnIntervals();
        Actor_Player p = playerScanner.Detect();

        if (p != null)
        {
            _bIsSearching = false;
            SwitchTarget(p.transform);
            HiveDictated = false;
        }

        IntervalTimer.PlayFromStart();
    }

    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
    }

    public override void OnPathCompleted()
    {
        Anim.SetTrigger("Attack");

        //if (currentTarget == Core) // Don't hate me for this XD
        //{
        //    Core.TakeDamage(Damage); 
        //}
    }

    //private void OnDrawGizmos()
    //{
    //    playerScanner.EditorGizmo(transform);
    //}
}
