using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpactDecalScript : MonoBehaviour
{
    //we will reference our materialChanger script to manipulate our emmision values.
    //over this objects lifetime we want to decrease the emmision value - the glowing property

    //when this is true we will manipulate our materials shader values
    bool activated = false;

    Material myMaterial;

    float myEmmisionValue;
    private void OnEnable()
    {
        activated = true;
        myMaterial = transform.GetComponent<MeshRenderer>().material; 
        
        MaterialHandler.materialFloatChanger(gameObject, 1.5f, "_EmissionIntensity");
        
        myEmmisionValue = myMaterial.GetFloat("_EmissionIntensity");
    }

    private void OnDisable()
    {
        activated = false;
    }

    private void Update()
    {
        if (activated)
        {
            MaterialHandler.materialFloatChanger(gameObject, myEmmisionValue -= Time.deltaTime , "_EmissionIntensity");
        }
    }
}
