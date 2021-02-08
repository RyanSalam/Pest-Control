using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool canFire = true; //if we can fire or not
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

    protected Coroutine currentCooldown;
    protected float currentRatio;
    private Timer cooldownDelayTimer;

    //this is set on our weapon script when we shoot
    //this will maybe be changed to if proj -> projFire() elseif raycast ->

    protected virtual void Awake()
    {
        player = LevelManager.Instance.Player;
        playerCam = player.PlayerCam;
        gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        //grab our muzzle flash component, 
        //and animator   
        muzzleFlashParticle = GetComponentInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();

        //initializing our weapon
        if (weaponAttachment != null)
            weaponAttachment.initialize(this);

        cooldownDelayTimer = new Timer(timeTillWeaponCooldown / 2, false);
        cooldownDelayTimer.OnTimerEnd += () => currentCooldown = StartCoroutine(WeaponCooldown(currentRatio));
    }

    protected virtual void Update()
    {
        cooldownDelayTimer.Tick(Time.deltaTime);

        if (auto && isFiring)
            PrimaryFire();
    }

    public virtual void Equip()
    {
        transform.SetParent(player.WeaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = player.WeaponHolder.localRotation;
        gameObject.SetActive(true);

        lastFired = 0.0f;

        HandleInput();
    }

    public virtual void HandleInput()
    {
        if (player == null)
            player = LevelManager.Instance.Player;

        if (player == null)
            player = FindObjectOfType<Actor_Player>();

        if (auto)
        {
            player.playerInputs.actions["Fire"].performed += (context) => isFiring = true;
        }

        else
        {
            player.playerInputs.actions["Fire"].performed += (context) => PrimaryFire();
        }
        
        player.playerInputs.actions["Fire"].canceled += (context) => Release();

        player.playerInputs.actions["Alt Fire"].started += (context) => SecondaryFire();

        //if (auto)
        //{
        //    if (Input.GetButton("Fire1"))
        //        PrimaryFire();
        //}

        //else
        //{
        //    if (Input.GetButtonDown("Fire1"))
        //        PrimaryFire();
        //}

        //if (Input.GetButtonUp("Fire1"))
        //    Release();

        //if (weaponAttachment != null)
        //{
        //    if (Input.GetButtonDown("Fire2"))
        //        SecondaryFire();
        //}
    }

    public virtual void Release()
    {
        isFiring = false;
        cooldownDelayTimer.PlayFromStart();
    }
    protected float GetHeatRatio()
    {
        return ((float)currentShots / (float)maxShots);
    }

    protected IEnumerator WeaponCooldown(float percentage)
    {
        float elapsed = timeTillWeaponCooldown * percentage;

        while (elapsed > 0)
        {
            
            elapsed -= Time.deltaTime;
            float p = maxShots * currentRatio;
            currentShots = Mathf.RoundToInt(p);

            //this line is for updating our UI, uiVar is a ui element
            LevelManager.Instance.WeaponUI.UpdateHeatBar(elapsed, timeTillWeaponCooldown);
            currentRatio = elapsed / timeTillWeaponCooldown;
            yield return null;
        }
        ResetWeaponStats(elapsed <= 0);
    }



    public virtual void Unequip()
    {

        if (auto)
        {
            player.playerInputs.actions["Fire"].performed -= (context) => isFiring = true;
        }

        else
        {
            player.playerInputs.actions["Fire"].performed -= (context) => PrimaryFire();


        }

        player.playerInputs.actions["Fire"].canceled -= (context) => Release();

        player.playerInputs.actions["Alt Fire"].started -= (context) => SecondaryFire();

        transform.SetParent(null);
        gameObject.SetActive(false);

      
    }

    public virtual void PrimaryFire()
    {
        if (currentCooldown != null)
            StopCoroutine(currentCooldown);

        lastFired = Time.time; //reset our last fired
        isFiring = true; //we are firing
        currentShots += shotIncrease; //increment our current shots
        LevelManager.Instance.WeaponUI.UpdateHeatBar((float)currentShots, (float)maxShots);
        currentRatio = GetHeatRatio();

        //should trigger our weapon overheating-breaking animation
        if (currentShots >= maxShots)
        {
            isFiring = false;
            canFire = false; //we cannot fire now
            //temporary coroutine until we get smarter - coroutine toggles our weapon variables
            currentCooldown = StartCoroutine(WeaponCooldown(GetHeatRatio()));
        }
    }

    public virtual void SecondaryFire()
    {
        if (LevelManager.Instance.CurrentEnergy >= weaponAttachment.energyCostPerFire)
        {
            LevelManager.Instance.CurrentEnergy -= weaponAttachment.energyCostPerFire;
            weaponAttachment.AltShoot();
        }
    }

    private void OnEnable()
    {
        LevelManager.Instance.WeaponUI.UpdateHeatBar((float)currentShots, (float)maxShots);

        if (currentCooldown != null)
        {
            StartCoroutine(WeaponCooldown(currentRatio));
        }
    }

    private void OnDisable()
    {
        if (currentCooldown != null)
            StopCoroutine(currentCooldown);
    }

    private void ResetWeaponStats(bool shouldReset)
    {
        if (shouldReset)
        {
            //reset our weapon variables 
            canFire = true;
            currentShots = 0;
            currentCooldown = null;
        }
    }
}


