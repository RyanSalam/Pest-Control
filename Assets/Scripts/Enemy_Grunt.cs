using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grunt : Actor_Enemy
{
    Scanner<Actor_Player> playerScanner;
    public bool hiveDictated;

    public int hitsRecieved;
    

    

    [Range(0, 360)] public float detectionAngle;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDestinationAroundTarget(CurrentDestination, AttackRange);
        Agent.speed = movementSpeed;
        Anim.SetBool("isDead", isDead);
        GetComponentInChildren<Enemy_AnimEvent>().OnEnable();
        hitsRecieved = 0;

        if (WaveManager.Instance.isBuildPhase == false)
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

        if (isDead) return;

        if (Vector3.Distance(transform.position, currentTarget.position) <= _attackRange * 1.5f)
        {
            //Vector3 dir = currentTarget.position - transform.position;
            //dir.y = 0;
            //dir = dir.normalized;
            //Quaternion rot = Quaternion.LookRotation(dir);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rot, -0.6f);

            Anim.SetFloat("DistanceToTarg", Vector3.Distance(CurrentTarget.transform.position, transform.position));

            // When attacking core
            if (currentTarget == Core.transform)
            {
                //Debug.Log("Attacking CORE");
                transform.LookAt(currentTarget);
            }
            // When attacking player
            else if (Vector3.Distance(transform.position, currentTarget.position) <= _attackRange)
            {
                //Debug.Log("Attacking player");
                Vector3 targetAim = new Vector3(currentTarget.position.x, currentTarget.position.y - 1.0f, currentTarget.position.z);
                transform.LookAt(targetAim);
            }
                
        }
        else
        {
            //Quaternion rot = Quaternion.LookRotation(Agent.velocity);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.6f);
            Quaternion rot = Quaternion.LookRotation(Agent.velocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, -0.6f);
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
        }

        IntervalTimer.PlayFromStart();
    }

    protected override void Death()
    {
        base.Death();
        Anim.SetBool("isDead", isDead);
        //gameObject.SetActive(false);
    }

    public override void OnPathCompleted()
    {
        Anim.SetTrigger("Attack");

        //if (currentTarget == Core) // Don't hate me for this XD
        //{
        //    Core.TakeDamage(Damage); 
        //}
    }

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        hitsRecieved++;
        Anim.SetInteger("hitsRecieved", hitsRecieved);
    }

    //private void OnDrawGizmos()
    //{
    //    playerScanner.EditorGizmo(transform);
    //}
}
