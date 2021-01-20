using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] protected LayerMask whatIsBuildable;
    
    public void Equip()
    {
      
    }
    public void PrimaryFire()
    {
        throw new System.NotImplementedException();
    }

    public bool PrimaryFireCheck()
    {
        return Input.GetButtonDown("Fire1");
    }

    public void SecondaryFire()
    {
        throw new System.NotImplementedException();
    }

    public bool SecondaryFireCheck()
    {
        return Input.GetButtonDown("Fire2");
    }

    public void Unequip()
    {
        throw new System.NotImplementedException();
    }
}
