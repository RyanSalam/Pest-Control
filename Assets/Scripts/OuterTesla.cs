using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterTesla : MonoBehaviour
{

    private Animator teslaAnim;
    [SerializeField] Animator trapAnimator; 
    // Start is called before the first frame update
    void Start()
    {
        teslaAnim = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        teslaAnim.SetBool("TisIdle", trapAnimator.GetBool("isIdle")); 
        teslaAnim.SetBool("TisAttacking", trapAnimator.GetBool("isAttacking")); 
    }
}
