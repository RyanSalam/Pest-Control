using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap_OL : MonoBehaviour
{

    private Animator laserAnim;
    [SerializeField] private Animator trapAnimator; 
    // Start is called before the first frame update
    private void Start()
    {
        laserAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        laserAnim.SetBool("LIsIdle", trapAnimator.GetBool("isIdle"));
        laserAnim.SetBool("LisAttacking", trapAnimator.GetBool("isAttacking")); 
    }

}
