using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] private GameObject trapToSpawn;
    [SerializeField] protected float offset;
    [SerializeField] protected float verticalSearch; //max distance for raycast
    [SerializeField] protected LayerMask whatIsBuildable; // detecting groundlayer for raycast 
    [SerializeField] protected int trapPrice;
    public bool isDebugging;
    
    public virtual void Awake()
    {
        //gameObject.SetActive(false);
    }

    public void Equip()
    {
        transform.SetParent(LevelManager.Instance.Player.WeaponHolder);
        transform.localPosition = Vector3.zero; //resetting gameObject's position
        gameObject.SetActive(true); //setting trap to activate when equiping 
    }

    private void Update()
    {
        transform.localPosition = new Vector3(offset, LevelManager.Instance.Player.transform.position.y, 0); //setting the offset of trap in front of player

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.localPosition, Vector3.down); //cast from trap to spawn 
        Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable);
        transform.localPosition = outHit.collider.transform.localPosition; // assigning the trap to raycast hit position
    }

    public void PrimaryFire()
    {
        //setting the trap GameObject to spawn on raycast's position 
        trapToSpawn.transform.position = transform.localPosition;
        trapToSpawn.SetActive(true);
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
}
