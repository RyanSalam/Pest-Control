using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Character")]
public class Character : ScriptableObject
{
    //Base Variables
    public Actor_Player player;


    //Character Select Screen
    public string c_name;
    public string c_desc;

    public Sprite A1_image;
    public string A1_text;
    public Sprite A2_image;
    public string A2_text;

    public Ability ab1;
    public Ability ab2;





}
