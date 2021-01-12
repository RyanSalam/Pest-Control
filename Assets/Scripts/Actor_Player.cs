using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Actor_Player : Actor
{
    protected CharacterController controller;

    [Header("Input Variables")]
    public bool controlsEnabled = true;
    public float mouseSensitivity = 100.0f;
    protected Vector2 moveVector;
    protected Vector2 mouseVector;
    [HideInInspector] public Vector2 externalMouseForce;
    private float _camRot = 0.0f;

    [SerializeField] protected float jumpStrength = 4.0f;
    [SerializeField] private float _minJumpDuration = 0.75f;
    private float _verticalVel = 0.0f;
    private bool _jumpRequest = false;
    private float _jumpElapsed = 0.0f;

    // Components
    [SerializeField] private Camera _playerCam;
    public Camera PlayerCam { get { return _playerCam; } }

    [SerializeField] private Ability _abilityOne;
    [SerializeField] private Ability _abilityTwo;

    public Ability AbilityOne { get { return _abilityOne; } }
    public Ability AbilityTwo { get { return _abilityTwo; } }


    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController>();
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
        HandleMovement();
        HandleRotation();
    }

    private void HandleInputs()
    {
        if (!controlsEnabled) return;

        // We're storing our mouse and movement inputs in vectors
        // Helps us know 
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        mouseVector.x = Input.GetAxis("Mouse X") + externalMouseForce.x * mouseSensitivity * Time.deltaTime;
        mouseVector.y = Input.GetAxis("Mouse Y") + externalMouseForce.y * mouseSensitivity * Time.deltaTime;

        _camRot -= mouseVector.y;
        _camRot = Mathf.Clamp(_camRot, -45f, 45f);

        // This allows us to control our jump
        // Meaning the longer we hold it, the higher we can jump
        if (Input.GetButtonDown("Jump") && controller.isGrounded) 
            _jumpRequest = true;

        if (Input.GetButtonUp("Jump"))
        {
            _jumpRequest = false;
        }

        if (AbilityOne != null && AbilityTwo != null)
        {
            if (Input.GetButtonDown(AbilityOne.AbilityButton) && AbilityOne.CanExecute())
                AbilityOne.Execute();

            if (Input.GetButtonDown(AbilityTwo.AbilityButton) && AbilityTwo.CanExecute())
                AbilityTwo.Execute();
        }


    }

    private void HandleMovement()
    {
        Vector3 movement = transform.right * moveVector.x + transform.forward * moveVector.y;
        movement *= moveSpeed;

        _verticalVel = controller.isGrounded ? -2.0f : _verticalVel + -9.81f * Time.deltaTime;

        // Condition to make sure we've let the jump button go
        // And we've at least covered some minimum ground 
        // Otherwise just tapping and letting it go would seem hella lame
        if (_jumpRequest || _jumpElapsed.IsWithin(Mathf.Epsilon, _minJumpDuration))
        {
            _jumpElapsed += Time.deltaTime;
            _verticalVel = (jumpStrength * _jumpElapsed) + (0.5f * -9.81f * _jumpElapsed * _jumpElapsed);
        }

        else
            _jumpElapsed = 0.0f;


        movement += Vector3.up * _verticalVel;

        controller.Move(movement * Time.deltaTime);

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
}
