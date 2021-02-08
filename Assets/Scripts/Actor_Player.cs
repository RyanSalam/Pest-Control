using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Actor_Player : Actor
{
    [Header("Input Variables")]
    public bool controlsEnabled = true;
    public float mouseSensitivity = 100.0f;
    protected Vector2 moveVector;
    protected Vector2 mouseVector;
    [HideInInspector] public Vector2 externalMouseForce;
    private float _camRot = 0.0f;

    [SerializeField] protected float jumpStrength = 4.0f;
    [SerializeField] private float _minJumpDuration = 0.75f;
    [SerializeField] private float _maxJumpDuration = 2.0f;
    private float _verticalVel = 0.0f;
    private bool _jumpRequest = false;
    private float _jumpElapsed = 0.0f;

    //physical attributes
    public float mass = 1f;

    //impact 
    Vector3 impactVector = Vector3.zero;
    bool isFlying = false;

    //states   
    public bool airtime = false;
    bool jumping = false;

    // Components
    [SerializeField] private Camera _playerCam;
    protected CharacterController controller;
    public Camera PlayerCam { get { return _playerCam; } }
    public CharacterController Controller { get { return controller; } }

    public PlayerInput playerInputs;

    [Header("Abilities")]
    [SerializeField] private Ability _abilityOne;
    [SerializeField] private Ability _abilityTwo;

    [SerializeField] protected Transform _abilitySpawnPoint;
    public Transform AbilitySpawnPoint { get { return _abilitySpawnPoint; } }
    public Ability AbilityOne { get { return _abilityOne; } }
    public Ability AbilityTwo { get { return _abilityTwo; } }

    [Header("Weapon")]
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Transform _trapHolder;
    [SerializeField] private IEquippable _currentEquiped;
    private int itemIndex = 0;

    public Transform WeaponHolder { get { return _weaponHolder; } }
    public Transform TrapHolder { get { return _trapHolder; } }
    public IEquippable CurrentEquipped { get { return _currentEquiped; } }

    //Audio Params
    public AudioCue _audioCue;
    public Character _cInfo;

    public AudioCue AudioCue
    {
        get { return _audioCue; }
    }
    public Character CharacterInfo
    {
        get { return _cInfo; }
    }

    protected override void Awake()
    {
        base.Awake();
        playerInputs = GetComponent<PlayerInput>();
        playerInputs.ActivateInput();

        controller = GetComponent<CharacterController>();

        playerInputs.actions["Jump"].started += (context) => HandleJump(true);
        playerInputs.actions["Jump"].canceled += (context) => HandleJump(false);

        playerInputs.actions["Weapon Switch"].performed += HandleWeaponSwap;

        _audioCue = GetComponent<AudioCue>();

        if (AbilityOne != null)
        {
            AbilityOne.Initialize(gameObject);
        }
            

        if (AbilityTwo != null)
            AbilityTwo.Initialize(gameObject);
    }



    protected override void Start()
    {
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected virtual void Update()
    {
        HandleInputs();
        HandleRotation();
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInputs()
    {
        if (!controlsEnabled) return;

        // We're storing our mouse and movement inputs in vectors
        // Helps us know 
        moveVector = playerInputs.actions["Move"].ReadValue<Vector2>();

        mouseVector = playerInputs.actions["Look"].ReadValue<Vector2>();
        mouseVector *= mouseSensitivity * Time.deltaTime;

        _camRot -= mouseVector.y;
        _camRot = Mathf.Clamp(_camRot, -45f, 45f);

        // This allows us to control our jump
        // Meaning the longer we hold it, the higher we can jump
           

        
        //if (Input.GetButtonUp("Jump"))
        //{
            
        //}

        //if (AbilityOne != null)
        //{
        //    AbilityOne.HandleInput();
        //}

        //if (AbilityTwo != null)
        //    AbilityTwo.HandleInput();



        //if (_currentEquiped != null)
        //    _currentEquiped.HandleInput();
    }

    private void HandleWeaponSwap(InputAction.CallbackContext context)
    {
        float mouseWheel = context.ReadValue<float>();
        if (mouseWheel != 0)
        {
            itemIndex += (int)mouseWheel;
            // We modulus it so we can never go above the max number of items we have in our inventory            
            itemIndex %= LevelManager.Instance.InventoryList.Count;
            var _currentItem = LevelManager.Instance.InventoryList[itemIndex];
            _currentItem.Use();
        }
    }

    protected void HandleJump(bool value)
    {
        if (value == true && controller.isGrounded)
        {
            _jumpElapsed += Time.fixedDeltaTime * 10;
            _jumpRequest = true;
        }

        else
        {
            _jumpRequest = false;
        }
    }

    private void DisableControls()
    {
        playerInputs.DeactivateInput();
        mouseVector = Vector2.zero;
        moveVector = Vector2.zero;
    }

    private void HandleMovement()
    {
        Vector3 movement = transform.right * moveVector.x + transform.forward * moveVector.y;
        movement *= moveSpeed * Time.fixedDeltaTime;

        _verticalVel = controller.isGrounded ? -0.5f : _verticalVel + -15f * Time.fixedDeltaTime;

        // Condition to make sure we've let the jump button go
        // And we've at least covered some minimum ground 
        // Otherwise just tapping and letting it go would seem hella lame
        if (_jumpRequest || _jumpElapsed.IsWithin(Mathf.Epsilon, _minJumpDuration) && _jumpElapsed <= _maxJumpDuration)
        {
            _jumpElapsed += Time.fixedDeltaTime;
            _verticalVel = (jumpStrength * _jumpElapsed * 2.3f) + (0.5f * -15f * _jumpElapsed * _jumpElapsed);
        }

        else
            _jumpElapsed = 0.0f;


        movement += Vector3.up * _verticalVel * Time.fixedDeltaTime;

        controller.Move(movement);
    }

    private void HandleRotation()
    {
        // We rotate the player Camera vertically using the cached camRot
        // We rotate the player horizontally using 
        PlayerCam.transform.localRotation = Quaternion.Euler(_camRot, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseVector.x);
    }

    protected virtual void Ondisable()
    {
        controlsEnabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        controlsEnabled = true;
    }

    public void EquipWeapon(IEquippable newWeapon)
    {
        if (newWeapon == CurrentEquipped) return;
        // We will first unequip our previous weapon
        // We then make our new weapon a child of our weaponHolder transform
        // then reset the local position so it snaps directly to it's parent position

        if (_currentEquiped != null)
            _currentEquiped.Unequip();

        newWeapon.Equip();
        _currentEquiped = newWeapon;
    }


    bool CheckSlope()
    {
        RaycastHit rayOut;
        bool checkRayHit = Physics.Raycast(transform.position, Vector3.down, out rayOut, 2f); // hardcoded to player's height; add variable later
        if (checkRayHit && rayOut.normal != Vector3.up)
            return true;
        return false;
    }
    public void AddImpact(float impactForce, Vector3 direction)
    {
        impactVector = Vector3.zero;
        _verticalVel = impactForce;
        isFlying = true;
        jumping = false;
        impactVector = direction;
        impactVector.Normalize();
        impactVector.y = 0f;
        impactVector *= impactForce * Mathf.Sin(Vector3.Angle(transform.up, direction)) / (0.5f * mass);
        _verticalVel = impactForce / mass;
        
    }

}
