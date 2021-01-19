using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] protected LayerMask whatIsBuildable;
    public void Equip()
    {
      
    }

    public bool PrimaryFire()
    {
        return true;
    }

    public bool RealeaseFire()
    {
        return true;
    }

    public bool SecondaryFire()
    {
        return true;
    }

    public virtual bool Buildable()
    {
        return true;
    }
    public void Build()
    {

    }
}
