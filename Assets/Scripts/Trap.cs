using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap : MonoBehaviour
{
    [SerializeField] protected int trapDamage = 1;
    [SerializeField] protected int maxUses = 10;
    [SerializeField] protected float buildDuration;
    [SerializeField] protected Color trapColor;
    //[SerializeField] protected bool isDying = false; 
    public event System.Action TrapDestroyed;
    [SerializeField] private GameObject trapDeathVFX; 

    [SerializeField] protected Image healthBar;
    [SerializeField] protected Color healthStartColor = Color.green;
    [SerializeField] protected Color healthEndColor = Color.red;
    public AudioCue audioPlayer;
    [SerializeField] AudioCueSO destroy;


    private int _currentUses = 0;
    public int CurrentUses
    {
        get { return _currentUses; }
        set 
        { 
            _currentUses = value;

            healthBar.fillAmount = 1 - (float)CurrentUses / (float)maxUses;
            Color lerpColor = Color.Lerp(healthStartColor, healthEndColor, 1 - healthBar.fillAmount);
            MaterialHandler.materialColorChanger(healthBar.material, lerpColor, "_EmissionColor");
        }
    }
    protected bool isTrapBuilt;
    Timer buildTimer;
    protected Animator anim;

    public Animator Anim { get{ return anim; } }

    
    public event System.Action OnDestroyed;

    protected virtual void Awake()
    {
        buildTimer = new Timer(buildDuration, false);
        buildTimer.OnTimerEnd += () => isTrapBuilt = true; //lamda expression: delegates without parameters and dont have an excessive function 
    }
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioCue>();

    }

    protected virtual void Update()
    {
        // checking if the trap is being built to false so that the build timer can take time during combat phase 
        if (isTrapBuilt == false) 
        {
            buildTimer.Tick(Time.deltaTime); //increasing build timer before the trap is built
            return;
        }

        //healthBar.fillAmount = 1 - (float)CurrentUses / (float)maxUses;
        Debug.Log("CurrentUses/maxUses: " + (float)CurrentUses / (float)maxUses);
    }

    public virtual void Activate()
    {
         if (!isTrapBuilt) // should be implemented on the top like this for other trap scripts if overiding
         {
             return;
         }
        
        // when trap is activated
        CurrentUses++; //add current uses

        if (CurrentUses >= maxUses)
        {
            Death();
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentUses += damage;

        if (CurrentUses >= maxUses)
        {
            Death();
        }
    }

    // Function to handle death methods
    void Death()
    {
        //LevelManager.Instance.AssessTraps(this);
        //isDying = true;
        Anim.SetTrigger("Destroy");
        audioPlayer.PlayAudioCue(destroy);
        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.SetTrigger("Destroy"); 
        }
        LevelManager.Instance.GetComponent<AudioCue>().PlayAudioCue(LevelManager.Instance.Char_SO.TrapDestroyed, 5);
    }

    protected virtual void OnEnable()
    {
        //function that gets called when a trap is placed
        if(WaveManager.Instance.isBuildPhase == false) // checking if it is not on the build phase 
        {
            buildTimer.PlayFromStart(); //starting buildTimer
        }
        else
        {
            isTrapBuilt = true; //if its on build phase instantly built the trap 
            //isDying = false; 
        }
        CurrentUses = 0; //this should always occur when you spawn a trap so that it resets its current uses and dosent destroy instantly
        LevelManager.Instance.AssessTraps(this); 
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
