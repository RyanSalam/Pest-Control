using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSender : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] PortalSender otherPortal;

    public bool isPortalPlaced = false;

    public bool playerCollision = false;
    public bool projectileCollision = false;

    private float forwardOffset;

    [SerializeField] int buildSpace = 1;
    [SerializeField] LayerMask whatIsBuildable;
    public bool isPortal = false;
    [HideInInspector] private float movementSpeed;

    public Transform bulletTransformPoint;
    public Transform portalForward;

    public Actor_Player actP;

    [SerializeField] LayerMask whatIsTargetable;
   
    void Update()
    {
        if (!gameObject.activeSelf)
            isPortalPlaced = false;
    }

    public void Use(Actor_Player user)
    {
        user.UnequipWeapon();
        //user.PortalHandler.EnterState(this);
    }

    public virtual bool Buildable()
    {
        Collider[] objs = Physics.OverlapBox(transform.position, Vector3.one * 0.4f);

        foreach (Collider col in objs)
        {
            if(col.GetComponent<Actor_Player>() == true && col != this.GetComponent<Collider>())
            {
                Debug.Log("Can't Build Portal");
                return false;
            }

        }
        Debug.Log("Can build Portal");
        return true;
    }

    public virtual void Build()
    {
        enabled = true;
        transform.parent = null;
        isPortalPlaced = true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
        Gizmos.color = Color.magenta;
    }

    public void HandlePlayerCollision()
    {
        Debug.Log("HandlePlayerCollision is functional");

        actP.Controller.enabled = false;
        actP.transform.position = otherPortal.destination.position;
        actP.transform.rotation = otherPortal.destination.rotation;
        actP.Controller.enabled = true;
        //triggerPortals(false);
        playerCollision = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (otherPortal.gameObject.activeSelf ==false || gameObject.activeSelf == false) { return; }

        if(other.CompareTag("Player"))
        {
            HandlePlayerCollision();
        }
    }

    public void HandleBulletCollision (Vector3 intDir)
    {
        //Ray mouseRay = playerCam.ScreenPointToRay(Input.mousePos);
        //RaycastHit hit;
        //if(Physics.Raycast(mouseRay, out hit, 80f, whatIsTargetable))
        //{
        //  DamageData damageData = new DamageData
        //     {
        //       damager = wielder,
        //       damageAmount = Attack.GetValue(),
        //       direction = firePoint.forward,
        //       damageSource = firePoint.position,
        //       enemyTarget = enemyHit,
        //     };
        //     enemyHit.TakeDamage(damageData);
        //     DamageDealt(damageData);
        //     Debug.log("Hit!" + hit.point);
    }

}
