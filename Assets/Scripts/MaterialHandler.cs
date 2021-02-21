using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialHandler
{
    //this script will be used for manipulating a game objects materials/shaders
    //creating a public function that other scripts can call, passing parameters to change said objects material properties 


    /// <summary>
    ///  This function can manipulate a gameobjects Color. Pass in: the current object you want to change, and the shaders Propertyname by doing - Shader.PropertyToID(the variable you want to change inside the shader graph) 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="color"></param>
    /// <param name="shaderValue"></param>
    /// <param name="shaderPropertyName"></param>
    public static void materialColorChanger(GameObject gameObject, Color color,  string shaderPropertyName)
    {
        
        //getting the objects renderer -> material
        Material existingMaterial = gameObject.GetComponent<Renderer>().material;

        if (existingMaterial != null) //we will only change the material if it exists in the first place
        {  
            //set our materials colour to the new desired one
            if (color != null)
                existingMaterial.SetColor(shaderPropertyName, color);
        }
    }



    /// <summary>
    /// This function can manipulate a gameobjects texture. Pass in: the current object you want to change, the desired new texture, and the shaders Propertyname by doing - Shader.PropertyToID(the variable you want to change inside the shader graph) 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="texture"></param>
    /// <param name="shaderPropertyName"></param>
    public static void materialTextureChanger(GameObject gameObject, Texture texture, string shaderPropertyName)
    {
        //getting the objects renderer -> material
        Material existingMaterial = gameObject.GetComponent<Renderer>().material;

        if (existingMaterial != null) //we will only change the material if it exists in the first place
        {
            //set our materials texture to the new desired one
            if (texture != null)
            existingMaterial.SetTexture(shaderPropertyName, texture);
        }

    }


    /// <summary>
    /// This function can manipulate a shaders float value. Pass in: the current object you want to change, the desired new float value, and the shaders Propertyname by doing - Shader.PropertyToID(the variable you want to change inside the shader graph) 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="shaderValue"></param>
    /// <param name="shaderPropertyName"></param>
    public static void materialFloatChanger(GameObject gameObject, float shaderValue, string shaderPropertyName)
    {
        //getting the objects renderer -> material
        Material existingMaterial = gameObject.GetComponent<Renderer>().material;

        if (existingMaterial != null) //we will only change the material if it exists in the first place
        {
            //setting the float will change the values of the shaders variables.  
            existingMaterial.SetFloat(shaderPropertyName, shaderValue);
        }

    }
}
