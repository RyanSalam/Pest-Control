using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Tornado Strike Ability")]
public class Ability_Tornado : Ability
{
    public Actor_Player pA;

    [SerializeField] private float tornadoStrikeForwardMomentum = 10f;
    [SerializeField] private float tornadoStrikeAoERange = 6f;
    [SerializeField] private float tornadoStrikeDamagePerSecond;

    [SerializeField] LayerMask whatisEnemy;
    [SerializeField] Collider[] enemiesHit;

    public override void Execute()
    {
        TornadoStrike();
    }

    public override bool CanExecute()
    {
        return base.CanExecute();
    }

    public override void Initialize(GameObject abilitySource)
    {
        pA = abilitySource.GetComponent<Actor_Player>();
        
    }

    public void TornadoStrike()
    {
        // Move forward
        pA.Controller.Move(pA.transform.forward * tornadoStrikeForwardMomentum * Time.deltaTime);
        // Detect enemies within the range
        enemiesHit = Physics.OverlapSphere(pA.transform.position, tornadoStrikeAoERange, whatisEnemy);

        if (enemiesHit.Length > 0)
        {
            // Execute AoEDamageSequence() 
            AoEDamageSequence();
        }
    }

    // The following function needs to be called on Update()?
    public void AoEDamageSequence()
    {

        var data = new DamageData
        {
            damageAmount = Mathf.RoundToInt(tornadoStrikeDamagePerSecond * Time.deltaTime),
            damager = pA,
        };

        foreach (Collider enemy in enemiesHit)
        {
            enemy.gameObject.GetComponent<Actor_Enemy>().TakeDamage(data);
            // Following line is for testing purposes
            //Debug.Log(enemy.name + ": " + enemy.gameObject.GetComponent<DamageBody>().CurrentHealth + " health");
        }

    }

}
