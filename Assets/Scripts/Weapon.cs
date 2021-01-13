using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //TODO: clean up variable names, look for more useful things we can add

    //This is our weapons base class,
    //This script should have all the variables you would need for weapons

    [Header("Weapon Stats")]
    //[SerializeField] protected Stat Attack;
    [SerializeField] protected float vRecoil = 0.0f;
    [SerializeField] protected float hRecoil = 0.0f;
    [SerializeField] protected float recoilSpeed = 1.0f;
    [SerializeField] protected float fireRate = 1f;
    //[SerializeField] public enum ammoType { projectilePrefab, RayCast, trapPrefab }; //using an enum to determine our ammo type, this way can use a switch on weapon scripts
    [Tooltip("Is the weapon automatic?")] [SerializeField] protected bool auto = false;

    
    [Space(10)]

    [Header("Weapon Cooldown Stats")]
    [SerializeField] public int shotCounter = 0;
    [SerializeField] protected int maxShots = 20;
    [SerializeField] protected int shotIncrease = 1;
    [SerializeField] public bool canFire = true;
    [SerializeField] protected bool isFiring = false;
    [SerializeField] protected float coolDownDelay = 3;
    [SerializeField] protected float shotReloadDelay = 3;

    [Space(10)]
    [Header("Weapon Efx")]
    protected AudioSource WeaponEfxPlayer;
    [SerializeField] protected AudioClip fireSound;

    //stats for our weapons damage
    public delegate void Weapondamage(DamageData data);
   // public event DamageData

    public float lastFired = 0.0f;
    protected Vector2 recoil;
    protected Vector3 initialCamPos;

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
    public bool ButtonPressed
    {
        get
        {
            if (auto) return Input.GetButton("Fire1");
            else return Input.GetButtonDown("Fire1");
        }
    }

   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Shoot()
    {
        //if these are not true we do not do anything, so nothing below will get run
        if (!(Time.time > fireRate + lastFired && canFire == true))
            return;

        //update our weapon variables
        lastFired = Time.time;
        isFiring = true;
        shotCounter += shotIncrease;

        //should trigger our weapon overheating-breaking animation
        if (shotCounter >= maxShots)
        {
            canFire = false;
            //temporary coroutine until we get smarter - coroutine toggles our weapon variables
            StartCoroutine(WeaponCooldown());
            
        }
           
    }

    public virtual void Release()
    {
        isFiring = false;
    }

    public void EquipWeapon()
    {
        lastFired = 0.0f;
    }

    public void UnEquipWeapon()
    {
        
    }

    protected float GetHeatRatio()
    {
        Debug.Log("Heat Ratio is: " + (float)shotCounter / (float)maxShots);
        return (float)shotCounter / (float)maxShots;

    }

    IEnumerator WeaponCooldown()
    {
        float elapsed = shotReloadDelay;

        canFire = false; //re-assurance

        while (elapsed > 0)
        {
            
            elapsed -= Time.deltaTime;

            //this line is for updating our UI, var is a ui element
             // uiVar = elapsed / shotReloadDelay;
            yield return null;
        }

        //reset our weapon variables 
        canFire = true;
        shotCounter = 0;
        Release(); //may not need this 
    }
}
