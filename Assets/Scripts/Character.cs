using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Character")]
public class Character : ScriptableObject
{
    //Character Select Screen
    public string c_name;
    public string c_desc;

    public Image A1_image;
    public string A1_text;
    public Image A2_image;
    public string A2_text;

    public Ability ab1;
    public Ability ab2;





}
