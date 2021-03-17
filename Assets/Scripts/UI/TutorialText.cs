using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class TutorialText : MonoBehaviour
{
    [SerializeField] GameObject keyboardText;
    [SerializeField] GameObject gamepadText;

    // FOR TEST PURPOSES ONLY
    //public bool usingGamepad = false;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.Player.playerInputs.onControlsChanged += UpdateText;

        bool isGamepad = LevelManager.Instance.Player.playerInputs.currentControlScheme == "Gamepad";
        if (isGamepad)
        {
            gamepadText.SetActive(true);
            keyboardText.SetActive(false);
        }
        else
        {
            gamepadText.SetActive(false);
            keyboardText.SetActive(true);
        }
    }



    private void UpdateText(PlayerInput obj)
    {
        bool isGamepad = obj.currentControlScheme == "Gamepad";
        if (isGamepad)
        {
            gamepadText.SetActive(true);
            keyboardText.SetActive(false);
        }
        else
        {
            gamepadText.SetActive(false);
            keyboardText.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        LevelManager.Instance.Player.playerInputs.onControlsChanged -= UpdateText;
    }

    //[ExecuteInEditMode]
    //void Update()
    //{
    //    if (usingGamepad)
    //    {
    //        gamepadText.SetActive(true);
    //        keyboardText.SetActive(false);
    //    }
    //    else
    //    {
    //        gamepadText.SetActive(false);
    //        keyboardText.SetActive(true);
    //    }
    //}

}
