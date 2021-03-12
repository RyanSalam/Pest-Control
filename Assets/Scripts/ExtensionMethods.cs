using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsWithin(this float value, float minNumber, float maxNumber)
    {
        return value >= minNumber && value <= maxNumber;
    }

    public static Vector3 AddY(this Vector3 value, float yNumber)
    {
        value.y += yNumber;
        return value;
    }
}
