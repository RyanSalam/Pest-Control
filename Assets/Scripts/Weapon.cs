using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
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
    protected float lastUnequiped = 0.0f;

    [Space(10)]
    [Header("Weapon Efx")]
    protected AudioSource WeaponEfxPlayer;
    [SerializeField] protected AudioClip fireSound;
    [SerializeField] protected ParticleSystem ImpactParticle;
    [SerializeField] protected ParticleSystem muzzleFlashParticle;
    [SerializeField] protected Color weaponColour;
    [SerializeField] protected GameObject[] objectsToChange;
    //stats for our weapons damage
    public delegate void Weapondamage(DamageData data);
    [SerializeField] public GameObject[] overheatSteamVFX;
   // public event DamageData

    public float lastFired = 0.0f;
    protected Vector3 recoil;
    protected Vector3 initialCamPos;

    public bool canFire = true; //if we can fire or not
    [SerializeField] public bool isFiring = false; //are we currently firing ?
    [SerializeField] protected AltFireAttachment weaponAttachment;
    [SerializeField] protected Transform firePoint;

    //our damage indicator prefabs
    [SerializeField] protected GameObject damageIndicatorObj;

    public delegate void DamageHandler(DamageData data);
    public event DamageHandler onDamageDealt;

    [Header("Reference Points:")]
    [SerializeField] protected Transform recoilPosition;
    [SerializeField] protected Transform rotationPoint;
    [Space(10)]

    [Header("Speed Settings:")]
    [SerializeField] protected float positionalRecoilSpeed = 8f;
    [SerializeField] protected float rotationalRecoilSpeed = 8f;
    [Space(10)]

    [SerializeField] protected float positionalReturnSpeed = 18f;
    [SerializeField] protected float rotationalReturnSpeed = 38f;
    [Space(10)]

    [Header("Amount Settings:")]
    [SerializeField] protected static Vector3 RecoilRotation = new Vector3(10, 5, 7);
    [SerializeField] protected Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

    //[Space(10)]
    //public Vector3 RecoilRotationAim = new Vector3(10, 4, 6);
    //public Vector3 RecoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);
    [Space(10)]

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    public void DamageDealt(DamageData data)
    {
        //calling all functions in the DamageHandler list
        onDamageDealt?.Invoke(data);
    }
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
    protected Timer cooldownDelayTimer;

    // WARNING: This bool is only used by the ChargeRifle!
    protected bool isCanceled = false;

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

        if(auto)
            animator.SetBool("isAuto", true);

        if (weaponColour != null)
        {
           for (int i = 0; i < objectsToChange.Length; i++)
            {
                MaterialHandler.materialColorChanger(objectsToChange[i], weaponColour, "_WeaponEmission");
            }
        }

        ObjectPooler.Instance.InitializePool(damageIndicatorObj, 5);

        //onDamageDealt += DamageIndication;
        overheatSteamVFX = GameObject.FindGameObjectsWithTag("gasLeakVFX");
    }

    protected virtual void Update()
    {
        cooldownDelayTimer.Tick(Time.deltaTime);

        if (auto && isFiring)
            PrimaryFire();
    }

    protected virtual void LateUpdate()
    {
        if (player == null) return;
        Vector2 inputDir = player.playerInputs.actions["Move"].ReadValue<Vector2>();
        animator.SetFloat("directionX", inputDir.x);
        animator.SetFloat("directionY", inputDir.y);

        animator.SetBool("isMoving", inputDir.magnitude > Mathf.Epsilon);
    }

    public virtual void Equip()
    {
        transform.SetParent(player.WeaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = player.WeaponHolder.localRotation;
        gameObject.SetActive(true);
        transform.DOLocalRotate(Vector3.zero, 0.3f).From(Vector3.right * -90);

        lastFired = 0.0f;

        // Registering inputs when we equip this.
        player.playerInputs.onActionTriggered += HandleInput;
    }

    public virtual void HandleInput(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Fire":
                // Couldn't find a simple hold for now. Handling automatic firing in update.
                if (context.phase == InputActionPhase.Started)
                    if (auto)
                        isFiring = true;

                    else
                        PrimaryFire();

                if (context.phase == InputActionPhase.Canceled)
                {
                    isCanceled = true;
                    Release();
                }

                break;

            case "Alt Fire":
                if(context.phase == InputActionPhase.Performed)
                {
                    if (weaponAttachment != null)
                        SecondaryFire();
                }
               
                break;
        }
    }

    public virtual void Release()
    {
        isFiring = false;
        cooldownDelayTimer.PlayFromStart();
        animator.SetTrigger("Release");
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
        // Deregistering inputs when we unequip this.
        player.playerInputs.onActionTriggered -= HandleInput;
        lastUnequiped = Time.time;
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

        Recoil();
        Debug.Log("Weapon Recoil");

        //setting animator parameters
        animator.SetTrigger("fire");
        animator.SetBool("isFiring" , isFiring);
        //should trigger our weapon overheating-breaking animation
        if (currentShots >= maxShots)
        {
            if (overheatSteamVFX.Length > 0) //play our vfx if they exist
                StartCoroutine(playVFX());
                //playOverHeatVFX();

            isFiring = false;
            canFire = false; //we cannot fire now
            animator.SetBool("isOverheating", true);
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
            animator.SetTrigger("SecondaryFire");
        }
    }

    private void OnEnable()
    {
        LevelManager.Instance.WeaponUI.UpdateHeatBar((float)currentShots, (float)maxShots);

        if (currentCooldown != null)
        {
            Debug.Log("Time.Time: " + Time.time + " lastUnequiped + timeTill: " + lastUnequiped + timeTillWeaponCooldown);
            if (Time.time > lastUnequiped + timeTillWeaponCooldown)
            {
                
                ResetWeaponStats(true);
            }

            else
            {
                StartCoroutine(WeaponCooldown(currentRatio));
            }
        }
    }

    private void OnDisable()
    {
        if (currentCooldown != null)
            StopCoroutine(currentCooldown);
    }

    private void OnDestroy()
    {
        onDamageDealt = null;
    }
    private void ResetWeaponStats(bool shouldReset)
    {
        if (shouldReset)
        {
            //reset our weapon variables 
            canFire = true;
            currentShots = 0;
            currentCooldown = null;
            animator.SetBool("isOverheating", false);
            LevelManager.Instance.WeaponUI.UpdateHeatBar(0, 1);
        }
    }
    public void Recoil()
    {
        if (isFiring)
        {
            //Debug.Log("recoiling");
            //weapon recoil script
            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

            recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
            Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
            rotationPoint.localRotation = Quaternion.Euler(Rot);

            //on shoot, not sure if this is the best location

            rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);

            //timeFiring += Time.deltaTime;


        }
        else //if we are not firing we need to go back to normal
        {
            //weapon recoil script
            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        }

        transform.localPosition = positionalRecoil;
        //transform.rotation = Quaternion.Euler(rotationalRecoil);
    }

    private void playOverHeatVFX()
    {
        Debug.Log("Playing overheatVFX");
        for (int i = 0; i < overheatSteamVFX.Length; i++)
        {
            overheatSteamVFX[i].GetComponent<ParticleSystem>().Play();
            
        }
    }

    IEnumerator playVFX()
    {
        yield return new WaitForSeconds(0.5f);
        playOverHeatVFX();

    }

    public void PlayAnimation(string animation)
    {
        animator.SetTrigger(animation);
    }
}



