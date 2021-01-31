using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasUI : MonoBehaviour
{
    public Character character;

    //Character Panel
    [SerializeField] TextMeshProUGUI charName;
    [SerializeField] TextMeshProUGUI charDesc;
    [SerializeField] Image A1_image;
    [SerializeField] TextMeshProUGUI A1_text;
    [SerializeField] Image A2_image;
    [SerializeField] TextMeshProUGUI A2_text;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnObjectOn(GameObject obj)
    {
        obj.SetActive(true);
        Debug.Log("clicked");
    }

    public void TurnObjectOff(GameObject obj)
    {    
            obj.SetActive(false);
    }

    public void Quit()
    {
        FindObjectOfType<GameManager>().OnQuitButton();
    }

    public void SelectCharacter(Character c)
    {
        character = c;
        charName.text = c.c_name;
        charDesc.text = c.c_desc;
        A1_image = c.A1_image;
        A1_text.text = c.A1_text;
        A2_image = c.A2_image;
        A1_text.text = c.A2_text;
        
    }

    
}
