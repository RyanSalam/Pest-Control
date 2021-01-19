using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class altFireAttachment : ScriptableObject
{
    protected Weapon weapon;

    [Space(10)]
    [Header("Alt Fire Stats")]
    [SerializeField] protected int energyCostPerFire = 50;
    [SerializeField] protected int secondaryDamage = 8;

    public abstract void AltShoot();

    public void initialize(Weapon weapon)
    {
        this.weapon = weapon;
    }
}

[CreateAssetMenu(fileName = "New ProjectileAttachment", menuName = "Attachment/Projectile")]
public class ProjectileAttachment : altFireAttachment
{
    [SerializeField] protected Rigidbody projectilePrefab;
    public override void AltShoot()
    {
        Instantiate(projectilePrefab, weapon.FirePoint.position, weapon.FirePoint.rotation);
    }
}


