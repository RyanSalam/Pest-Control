using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialTest : MonoBehaviour
{
    Material myMaterial;
    string[] propertyIDs;
    int[] propertyIDints;


    public Texture textureTest;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        propertyIDs = myMaterial.GetTexturePropertyNames();
        propertyIDints = myMaterial.GetTexturePropertyNameIDs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //you can use the foreach loops to determine what properties your shader has(strings)
             foreach (string s in propertyIDs)
            {
                Debug.Log("Property id: " + s);
            }

            MaterialHandler.materialColorChanger(gameObject, Color.blue, "_BaseColor");
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            //you can use the foreach loops to determine what properties ID's your shader has (ints)
            //foreach(int i in propertyIDints)
            //{
            //    Debug.Log("propertIDint: " + i);
            //}

            MaterialHandler.materialColorChanger(gameObject, Color.cyan, "_BaseColor");
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            if (textureTest != null)
            MaterialHandler.materialTextureChanger(gameObject, textureTest, "_BaseMap");
        }
    }
}