using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageData
{
    public int damageAmount;
    public Actor damager;
    public Actor damagedActor;
    public Vector3 direction;
    public Vector3 damageSource;
    public Vector3 hitNormal;
}
