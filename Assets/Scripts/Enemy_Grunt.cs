using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grunt : Actor_Enemy
{
    Scanner<Actor_Player> playerScanner;

    protected override void Start()
    {
        base.Start();

        playerScanner = new Scanner<Actor_Player>(transform);
        playerScanner.targetMask = _playerLayer;

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
        }

        IntervalTimer.PlayFromStart();
    }

    protected override void Death()
    {
        base.Death();
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
}
