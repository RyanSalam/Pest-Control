using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grunt : Actor_Enemy
{
    Scanner<Actor_Player> playerScanner;

    [Range(0, 360)] public float detectionAngle;

    protected override void Start()
    {
        base.Start();

        // Initialising the player scanner properly passing relevant variables
        playerScanner = new Scanner<Actor_Player>(transform);
        playerScanner.targetMask = _playerLayer;
        playerScanner.detectionRadius = AttackRange * 2;
        playerScanner.detectionAngle = detectionAngle;

        currentTarget = LevelManager.Instance.Core.transform;
        if (GetRandomPointAroundTarget(currentTarget.position, _attackRange, out currentDestination))
        {
            Agent.SetDestination(currentDestination);
        }
    }

    protected override void Update()
    {
        base.Update();

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.6f);
    }

    public override void OnBtwnIntervals()
    {
        base.OnBtwnIntervals();
        Actor_Player p = playerScanner.Detect();

        if (p != null)
        {
            _bIsSearching = false;
            SwitchTarget(p.transform);
            EnemyManager.Instance.ReassessGrunts(this);
            EnemyManager.Instance.RegisterGrunt(this);
        }

        IntervalTimer.PlayFromStart();
    }

    protected override void Death()
    {
        base.Death();
        EnemyManager.Instance.ReassessGrunts(this);
        Destroy(gameObject);
    }

    public override void OnPathCompleted()
    {
        Anim.SetTrigger("Attack");

        //if (currentTarget == Core) // Don't hate me for this XD
        //{
        //    Core.TakeDamage(Damage); 
        //}
    }

    private void OnDrawGizmos()
    {
        playerScanner.EditorGizmo(transform);
    }
}
