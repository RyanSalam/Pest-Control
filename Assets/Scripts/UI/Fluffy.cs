using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Fluffy : MonoSingleton<Fluffy>
{
    public event System.Action<string> onTextChange;

    [SerializeField] TMP_Text dialogue;
    [SerializeField] string[] defaultTexts;

    private string _textToOverride;
    public string TextToOverride
    {
        get { return _textToOverride; }
        set 
        {
            _textToOverride = value;
            onTextChange?.Invoke(_textToOverride);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogue.text = defaultTexts[0];
        onTextChange += HandleDialogue;
    }

    // Update is called once per frame
    void Update()
    {
        //HandleDialogue();
    }

    private void HandleDialogue(string overridingText)
    {
        if (overridingText != "")
        {
            dialogue.text = overridingText;
        }
        else
        {
            //dialogue.text = defaultTexts[0];
            dialogue.text = defaultTexts[UnityEngine.Random.Range(0, defaultTexts.Length)];
        }
    }


}
