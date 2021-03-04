using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessHandler : MonoBehaviour
{
    [SerializeField] Volume volume;

    public void SetVolumeWeight(float weight)
    {
        volume.weight = weight;
    }
}
