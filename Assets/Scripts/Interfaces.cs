using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
     void Equip();

     bool PrimaryFireCheck();

     bool SecondaryFireCheck();

     void SecondaryFire();

     void PrimaryFire();
    
}