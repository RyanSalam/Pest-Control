using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject mainPanel;

    [SerializeField] InputAction inputActions;


    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.performed += HandleInput;
    }

    private void OnDisable()
    {
        inputActions.performed -= HandleInput;
        inputActions.Disable();
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        creditsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }



}
