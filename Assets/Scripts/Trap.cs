using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected int trapDamage = 1;
    [SerializeField] protected int maxUses = 10;
    [SerializeField] protected float buildDuration;
    protected int currentUses;
    protected bool isTrapBuilt;
    Timer buildTimer;
    protected Animator anim;



    protected virtual void Awake()
    {
        buildTimer = new Timer(buildDuration, false);
        buildTimer.OnTimerEnd += () => isTrapBuilt = true; //lamda expression: delegates without parameters and dont have an excessive function 
    }
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        // checking if the trap is being built to false so that the build timer can take time during combat phase 
        if (isTrapBuilt == false) 
        {
            buildTimer.Tick(Time.deltaTime); //increasing build timer before the trap is built
            return;
        }
        
    }

    public virtual void Activate()
    {
        /* if (!isTrapBuilt) // should be implemented on the top like this for other trap scripts if overiding
         {
             return;
         }
        */
        // when trap is activated
        currentUses++; //add current uses 
        if (currentUses >= maxUses) //checks if the current trap uses is greater or equal to max
        {
            gameObject.SetActive(false);  //setting the game object to false 
        }
    }

    protected virtual void OnEnable()
    {
        //function that gets called when a trap is placed
        if(WaveManager.Instance.IsBuildPhase == false) // checking if it is not on the build phase 
        {
            buildTimer.PlayFromStart(); //starting buildTimer
        }
        else
        {
            isTrapBuilt = true; //if its on build phase instantly built the trap 
        }
        currentUses = 0; //this should always occur when you spawn a trap so that it resets its current uses and dosent destroy instantly 

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
        Gizmos.color = Color.red;
    }

    public virtual void Interact()
    {
        //player energy refund here
    }
}
