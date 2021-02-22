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
    public Sprite c_profile;

    public Sprite A1_image;
    public string A1_name;
    public string A1_text;
    public Sprite A2_image;
    public string A2_name;
    public string A2_text;

    public Ability ab1;
    public Ability ab2;


    [Header("Dialogue")]

    public AudioCueSO CharacterChosen;

    public AudioCueSO BuildPhaseStart;
    public AudioCueSO WaveStart;

    public AudioCueSO CoreDamaged;
    public AudioCueSO UseAbility1;
    public AudioCueSO UseAbility2;
    public AudioCueSO EnemyKill;
    public AudioCueSO PlayerHit;
    public AudioCueSO PlayerRespawn;
    public AudioCueSO PurchaseItem;
    public AudioCueSO BuildTrap;
    public AudioCueSO TrapDestroyed;
    
    public AudioCueSO MissionStart;
    public AudioCueSO MissionLoss;
    public AudioCueSO MissionWin;

    public AudioCueSO Responses;
}
