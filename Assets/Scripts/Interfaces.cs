using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
     void Equip();
     bool PrimaryFire();
     bool RealeaseFire();
     bool SecondaryFire();
}