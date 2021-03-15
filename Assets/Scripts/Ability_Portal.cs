using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Portal Ability")]
public class Ability_Portal : Ability
{
    public GameObject prefabToSpawn;
    private Actor_Player pA;

    List<PortalSender> portalList;
    private int portalCount = 0;
    public bool canMake;
    [SerializeField] protected LayerMask whatIsBuildable;// detecting groundlayer for raycast 
    [SerializeField] protected LayerMask obstacleMasks;
    [SerializeField] private float obstacleDetectionRange = 3;
    [SerializeField] protected float verticalSearch; //max distance for raycast
    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);

        // Get the placeholder child object from Actor_Player
        // Disable both portals on init
        pA = abilitySource.GetComponent<Actor_Player>();
        GameObject portalPrefab = Instantiate(prefabToSpawn, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
        portalList = new List<PortalSender>();

        //portalArray = portalPrefab.GetComponentsInChildren<Transform>();
        foreach (PortalSender t in portalPrefab.GetComponentsInChildren<PortalSender>())
        {
            t.transform.SetParent(null);
            if (t != portalPrefab.transform)
            {
                portalList.Add(t);
                //t.GetComponent<PortalSender>().actP = pA;
            }
        }

        portalPrefab.gameObject.SetActive(false);
        portalCount = 0;
        foreach (PortalSender portal in portalList)
        {
            portal.gameObject.SetActive(false);
        }
    }

    public override void Execute()
    {

        RaycastHit outHit;
        Ray floorCast = new Ray(pA.AbilitySpawnPoint.position, Vector3.down); //cast from player to spawn 
        Debug.DrawRay(pA.AbilitySpawnPoint.position, Vector3.down, Color.blue);

        Collider[] obstacles = Physics.OverlapBox(pA.AbilitySpawnPoint.position, Vector3.one * obstacleDetectionRange, Quaternion.identity, obstacleMasks);

        /*
        if (Physics.Raycast(floorCast, out outHit, verticalSearch, whatIsBuildable))
        {
            pA.AbilitySpawnPoint.position = outHit.transform.position;

        }
        */
        canMake = obstacles.Length <= 0;

        if (canMake)
        {
            portalList[portalCount].transform.position = pA.AbilitySpawnPoint.position + pA.AbilitySpawnPoint.transform.forward * 3;
            portalList[portalCount].transform.rotation = pA.AbilitySpawnPoint.rotation;
            portalList[portalCount].gameObject.SetActive(true);
            portalCount++;
            player.CurrentEquipped.GetAnimator().SetTrigger("Ability");
            player.CurrentEquipped.GetAnimator().SetInteger("AbilityIndex", AbilityIndex);
        }
        
        if (portalCount == portalList.Count)
            // Call this when you want to start the cooldown
            base.Execute();
    }

    public override bool CanExecute()
    {
        return portalCount < portalList.Count && pA.Controller.isGrounded;
    }

    public override void OnCooldownEnd()
    {
        // Reset portals
        portalCount = 0;
        
    }

    public override void OnLifetimeEnd()
    {
        foreach (PortalSender portal in portalList)
        {
            portal.gameObject.SetActive(false);
        }
    }
}
