using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{

    // Wave management
    [SerializeField] Text waveNumber;
    // Update wave number - called on a new wave
    /// <summary>
    /// Update the current wave. Call at start of a new wave. Takes in current wave number
    /// </summary>
    public void UpdateWaveNumber(int wave)
    {
        waveNumber.text = wave.ToString();
    }

}
