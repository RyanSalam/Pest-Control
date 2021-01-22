using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IEquippable
{

    //TODO: clean up variable names, look for more useful things we can add

    //This is our weapons base class,
    //This script should have all the variables you would need for weapons

    [Header("Weapon Stats")]
    //[SerializeField] protected Stat Attack;
    [SerializeField] protected float vRecoil = 0.0f;
    [SerializeField] protected float hRecoil = 0.0f;
    [SerializeField] protected float recoilSpeed = 1.0f;
    [SerializeField] protected float fireRate = 1.0f;
    [SerializeField] protected int Damage = 1;
    //[SerializeField] public enum ammoType { projectilePrefab, RayCast, trapPrefab }; //using an enum to determine our ammo type, this way can use a switch on weapon scripts
    [Tooltip("Is the weapon automatic?")] [SerializeField] protected bool auto = false;
  

    [Space(10)]

    [Header("Weapon Cooldown Stats")]
    [SerializeField] public int currentShots = 0; //current shots keep track of how many shots we have fired (resets after cooldown) 
    [SerializeField] protected int maxShots = 20; //maxShots is the maximimun number of shots a gun can shoot before it breaks (currentshots > maxShots -> cooldown)
    [SerializeField] protected int shotIncrease = 1; //everytime you shoot the currentShots is increased by shotIncrease, allowing for some guns to overheat faster
    //[SerializeField] protected float coolDownDelay = 3; //
    [SerializeField] protected float timeTillWeaponCooldown = 3; //how long it takes for our gun to reload (cool off)

    [Space(10)]
    [Header("Weapon Efx")]
    protected AudioSource WeaponEfxPlayer;
    [SerializeField] protected AudioClip fireSound;
    [SerializeField] protected ParticleSystem ImpactParticle;
    [SerializeField] protected ParticleSystem muzzleFlashParticle;

    //stats for our weapons damage
    public delegate void Weapondamage(DamageData data);
   // public event DamageData

    public float lastFired = 0.0f;
    protected Vector3 recoil;
    protected Vector3 initialCamPos;

    [HideInInspector] public bool canFire = true; //if we can fire or not
    [SerializeField] protected bool isFiring = false; //are we currently firing ?
    [SerializeField] protected AltFireAttachment weaponAttachment;
    [SerializeField] protected Transform firePoint;

    public Transform FirePoint
    {
        get { return firePoint; }
    }

   //our player
   protected Actor_Player player;
    public Actor_Player Player
    {
        get { return player; }
    }

    protected Animator animator;
    public Animator Animator
    {
        get { return animator; }
        
    }

    //our reference to the players Camera
    protected Camera playerCam;

    //this is set on our weapon script when we shoot
    //this will maybe be changed to if proj -> projFire() elseif raycast ->

    protected virtual void Awake()
    {
        player = LevelManager.Instance.Player;
    }


    protected virtual void Start()
    {
        
        playerCam = player.PlayerCam;
        //grab our muzzle flash component, 
        //and animator   
        muzzleFlashParticle = GetComponentInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();

        //initializing our weapon
        if (weaponAttachment != null)
            weaponAttachment.initialize(this);
    }

    protected virtual void Update()
    {
        if (isFiring)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                Debug.Log("Button is released.");
                Release();
            }
                
        }
    }

    public virtual void Shoot()
    {
        //if these are not true we do not do anything, so nothing below will get run
        if (!(Time.time > fireRate + lastFired && canFire == true))
            return;

        //update our weapon variables
        lastFired = Time.time; //reset our last fired
        isFiring = true; //we are firing
        currentShots += shotIncrease; //increment our current shots

        //should trigger our weapon overheating-breaking animation
        if (currentShots >= maxShots)
        {
            canFire = false; //we cannot fire now
            //temporary coroutine until we get smarter - coroutine toggles our weapon variables
            StartCoroutine(WeaponCooldown());
            
        }
           
    }


    public virtual void Release()
    {
        isFiring = false;
    }
    protected float GetHeatRatio()
    {
        Debug.Log("Heat Ratio is: " + (float)currentShots / (float)maxShots);
        return (float)currentShots / (float)maxShots;

    }

    IEnumerator WeaponCooldown()
    {
        float elapsed = timeTillWeaponCooldown;

        canFire = false; //re-assurance

        while (elapsed > 0)
        {
            
            elapsed -= Time.deltaTime;

            //this line is for updating our UI, uiVar is a ui element
             // uiVar = elapsed / shotReloadDelay;
            yield return null;
        }

        //reset our weapon variables 
        canFire = true;
        currentShots = 0;
        Release(); //may not need this 
    }

    public virtual void Equip()
    {
        transform.SetParent(player.WeaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = player.WeaponHolder.localRotation;
        gameObject.SetActive(true);

        lastFired = 0.0f;
    }

    public virtual void Unequip()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public virtual bool PrimaryFireCheck()
    {
        if (auto) return Input.GetButton("Fire1");
        else return Input.GetButtonDown("Fire1");
    }
    public virtual void PrimaryFire()
    {
        //if these are not true we do not do anything, so nothing below will get run
        if (!(Time.time > fireRate + lastFired && canFire == true))
            return;

        //update our weapon variables
        lastFired = Time.time; //reset our last fired
        isFiring = true; //we are firing
        currentShots += shotIncrease; //increment our current shots

        //should trigger our weapon overheating-breaking animation
        if (currentShots >= maxShots)
        {
            canFire = false; //we cannot fire now
            //temporary coroutine until we get smarter - coroutine toggles our weapon variables
            StartCoroutine(WeaponCooldown());

        }
    }

    public virtual bool SecondaryFireCheck()
    {
        return Input.GetButtonDown("Fire2");
    }

    public virtual void SecondaryFire()
    {
        //if (player.energy > weaponAttachment.energyCost)
            //player.energy -=  weaponAttachment.energyCost
        weaponAttachment.AltShoot();
  
    }
}


