using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
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

    

    
}
