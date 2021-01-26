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

    public override void Execute()
    {

        portalList[portalCount].transform.position = pA.AbilitySpawnPoint.position + pA.AbilitySpawnPoint.transform.forward * 3;
        portalList[portalCount].transform.rotation = pA.AbilitySpawnPoint.rotation;

        portalList[portalCount].gameObject.SetActive(true);
        portalCount++;

        if (portalCount == portalList.Count)
            // Call this when you want to start the cooldown
            base.Execute();
    }

    public override bool CanExecute()
    {
        return portalCount < portalList.Count;
    }

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

    public override void OnCooldownEnd()
    {
        // Reset portals
        portalCount = 0;
        foreach (PortalSender portal in portalList)
        {
            portal.gameObject.SetActive(false);
        }
    }
}
