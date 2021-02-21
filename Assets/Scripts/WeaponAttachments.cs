using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AltFireAttachment : ScriptableObject
{
    protected Weapon weapon;

    [Space(10)]
    [Header("Alt Fire Stats")]
    [SerializeField] public int energyCostPerFire = 50;
    [SerializeField] protected int secondaryDamage = 8;

    public abstract void AltShoot();

    public void initialize(Weapon weapon)
    {
        this.weapon = weapon;
    }
}


