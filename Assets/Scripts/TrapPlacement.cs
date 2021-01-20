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
    public void Equip()
    {
        gameObject.SetActive(true); //setting trap gameobject to active when equiping 
    }

    public virtual void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = new Vector3(offset, LevelManager.Instance.Player.transform.position.y, 0); //setting offset in front of the player

        RaycastHit outHit;
        Ray floorCast = new Ray(transform.position, Vector3.down); // cast from trapObject to spawn 
        Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable);
        transform.position = outHit.collider.transform.position; // assigns the trap to where the raycast hits
    }

    public void PrimaryFire()
    {
        //setting the trapGameobject to spawn on where the new position is located by raycast
        trapToSpawn.transform.position = transform.position;
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
}
