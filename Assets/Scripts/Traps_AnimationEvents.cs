using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps_AnimationEvents : MonoBehaviour
{
    public GameObject trapParent; 
    public void Despawn()
    { 
        trapParent.SetActive(false);
        LevelManager.Instance.AssessTraps(trapParent.GetComponent<Trap>());
    }
}
