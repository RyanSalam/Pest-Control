using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 5f;
    Vector2 startPos;
    Material myMat;

    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<Renderer>().material;
        startPos = new Vector2(scrollSpeed, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        myMat.mainTextureOffset += startPos * Time.deltaTime;    
    }
}
