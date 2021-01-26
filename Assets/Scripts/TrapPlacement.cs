using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] private GameObject trapToSpawn;
    public Transform trapModel;
    [SerializeField] protected float offset;
    [SerializeField] protected float verticalSearch; //max distance for raycast
    [SerializeField] protected LayerMask whatIsBuildable; // detecting groundlayer for raycast 
    [SerializeField] protected int trapPrice;
    public bool isDebugging;


    //Audio Settings
    AudioCue ACue;

    public virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Equip()
    {
        transform.SetParent(LevelManager.Instance.Player.TrapHolder);
        transform.localPosition = Vector3.zero + transform.forward * offset; //resetting gameObject's position
        gameObject.SetActive(true); //setting trap to activate when equiping 
    }

    private void Update()
    {
        //transform.position = new Vector3(offset, LevelManager.Instance.Player.transform.position.y, 0); //setting the offset of trap in front of player

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.position, Vector3.down); //cast from trap to spawn 
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        if (Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable))
        {
           trapModel.position = outHit.transform.position; // assigning the trap to raycast hit position
        }
       
    }

    public void PrimaryFire()
    {
        //setting the trap GameObject to spawn on raycast's position 
        GameObject tempTrap = Instantiate(trapToSpawn, trapModel.position, transform.rotation); //instantiating trap 
        ACue.PlayAudioCue();
    }

    public bool PrimaryFireCheck()
    {
        return Input.GetButtonDown("Fire1");
    }

    public void SecondaryFire()
    {
        transform.Rotate(transform.rotation.eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles); //rotating trap by 90 degrees clockwise 
    }

    public bool SecondaryFireCheck()
    {
        return Input.GetButtonDown("Fire2");
    }

    public void Unequip()
    {
        gameObject.SetActive(false); //setting trap to deActivate when unEquipping 
    }

    protected void OnDrawGizmos()
    {
        if(isDebugging)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - verticalSearch, transform.position.z), Color.green);
        }
    }

    protected void Start()
    {
        ACue = GetComponent<AudioCue>();
    }
}
