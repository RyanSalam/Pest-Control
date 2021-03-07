using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Fluffy : MonoSingleton<Fluffy>
{
    [SerializeField] TMP_Text dialogue;
    [SerializeField] string[] defaultTexts;

    public string textToOverride = ""; 

    // Start is called before the first frame update
    void Start()
    {
        dialogue.text = defaultTexts[0];
    }

    // Update is called once per frame
    void Update()
    {
        HandleDialogue();
    }

    private void HandleDialogue()
    {
        if (textToOverride != "")
        {
            dialogue.text = textToOverride;
        }
        else
        {
            dialogue.text = defaultTexts[0];
        }
    }

}
