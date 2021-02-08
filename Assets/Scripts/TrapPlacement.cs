using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] private GameObject trapToSpawn;
    public Transform trapModel;
    [SerializeField] protected float offset;
    [SerializeField] protected float verticalSearch; //max distance for raycast
    [SerializeField] protected LayerMask whatIsBuildable;// detecting groundlayer for raycast 
    [SerializeField] protected LayerMask obstacleMasks;
    [SerializeField] protected int trapPrice;
    public bool isDebugging;
    public Material Hologram;
    public bool CanPlace;

    [SerializeField] private float obstacleDetectionRange = 3;

    Actor_Player player;

    //Audio Settings
    AudioCue ACue;

    public virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    protected void Start()
    {
        ACue = GetComponent<AudioCue>();
    }

    private void Update()
    {

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.position, Vector3.down); //cast from trap to spawn 
        Debug.DrawRay(transform.position, Vector3.down, Color.green);

        Collider[] obstacles = Physics.OverlapBox(trapModel.position, Vector3.one * obstacleDetectionRange, Quaternion.identity, obstacleMasks);

        if (Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable))
        {
            trapModel.position = outHit.transform.position;
            
        }

        CanPlace = obstacles.Length <= 0;

        if (CanPlace)
        {
            trapModel.position = outHit.transform.position;
            Hologram.SetColor("Color_F3B47044", Color.blue); //assigning placeable trap colour to blue 
        }
        else
        { 
            
            Hologram.SetColor("Color_F3B47044", Color.red);// Changing the colour of the trap to red if it can't be placed
            CanPlace = false;
        }
        
    }

    public virtual void HandleInput()
    {
        player = LevelManager.Instance.Player;

        player.playerInputs.actions["Fire"].started += (context) => PlaceTrap();
        player.playerInputs.actions["Alt Fire"].started += (context) => RotateTrap();
    }

    protected void PlaceTrap()
    {
        //setting the trap GameObject to spawn on raycast's position IF its on whatIsbuildable
        if (CanPlace)
        {
            GameObject tempTrap = Instantiate(trapToSpawn, trapModel.position, transform.rotation); //instantiating trap 
            ACue.PlayAudioCue();
        }
    }

    public void RotateTrap()
    {
        transform.Rotate(transform.rotation.eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles); //rotating trap by 90 degrees clockwise 
    }

    public void Equip()
    {
        transform.SetParent(LevelManager.Instance.Player.TrapHolder);
        transform.localPosition = Vector3.zero + transform.forward * offset; //resetting gameObject's position
        gameObject.SetActive(true); //setting trap to activate when equiping 
    }

    public void Unequip()
    {
        gameObject.SetActive(false); //setting trap to deActivate when unEquipping 

        player.playerInputs.actions["Fire"].started -= (context) => PlaceTrap();
        player.playerInputs.actions["Alt Fire"].started -= (context) => RotateTrap();
    }

    protected void OnDrawGizmos()
    {
        if(isDebugging)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - verticalSearch, transform.position.z), Color.green);
        }

        Gizmos.DrawWireCube(trapModel.position, Vector3.one * obstacleDetectionRange);
    }
}
