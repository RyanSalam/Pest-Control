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

    public Transform bulletTransformPoint;

    public Actor_Player actP;

   
    void Update()
    {

    }


    protected virtual void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
        //Gizmos.color = Color.magenta;
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
