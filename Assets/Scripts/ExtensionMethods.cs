using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsWithin(this float value, float minNumber, float maxNumber)
    {
        return value >= minNumber && value <= maxNumber;
    }
}
