using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacement : MonoBehaviour, IEquippable
{
    [SerializeField] private GameObject trapToSpawn;
    [SerializeField] protected float offset;
    [SerializeField] protected float verticalSearch; // max distanace for raycast 
    [SerializeField] protected LayerMask whatIsBuildable; // detecting groundlayer for raycast 
    [SerializeField] protected int trapPrice;
    public bool isDebuging;
  
    public virtual void Awake()
    {
        //gameObject.SetActive(false);
    }

    public void Equip()
    {
        transform.SetParent(LevelManager.Instance.Player.WeaponHolder);
        transform.localPosition = Vector3.zero; //ressetting position
        gameObject.SetActive(true); //setting trap gameobject to active when equiping 
    }

    private void Update()
    {
        transform.localPosition = new Vector3(offset, LevelManager.Instance.Player.transform.position.y, 0); //setting offset in front of the player

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.localPosition, Vector3.down); // cast from trapObject to spawn 
        Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable);
        transform.localPosition = outHit.collider.ClosestPointOnBounds(outHit.point); // assigns the trap to where the raycast hits
    }

    public void PrimaryFire()
    {
        //setting the trapGameobject to spawn on where the new position is located by raycast
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
        gameObject.SetActive(false);
    }

    protected void OnDrawGizmos()
    {
        if(isDebuging)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - verticalSearch, transform.position.z), Color.green);
        }
    }
}
