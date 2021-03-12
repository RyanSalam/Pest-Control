using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class PostProcessHandler : MonoBehaviour
{
    [SerializeField] Volume volume;

    public void SetVolumeWeight(float weight)
    {
        volume.weight = weight;
    }

    public Tween SetVolumeWeight(float start, float end, float duration)
    {
        return DOVirtual.Float(start, end, duration, SetVolumeWeight);
    }
}
