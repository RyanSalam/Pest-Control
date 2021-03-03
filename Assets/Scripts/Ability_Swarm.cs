using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "SwarmAbility", menuName = "Abilities/Swarm Ability")]
public class Ability_Swarm : Ability
{
    [SerializeField] public float swarmMaxcharge = 2f;
    [SerializeField] public int maxNanoDrones = 10;
    [SerializeField] public float nanoDroneDamage = 10f;
    [SerializeField] public float nanoDroneAttackRadius = 10f;
    [SerializeField] public float nanoDroneLaunchDelay = 0.1f;
    [SerializeField] public float nanoDroneMovementSpeed = 1f;
    [SerializeField] public float nanoDroneRotationSpeed = 1f;
    bool firing = false;


    //[SerializeField] Text temp;

    public int currentDrones;

    private Actor_Player pA;

    public GameObject swarmMaster;

    GameObject swarmController;

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        currentDrones = maxNanoDrones;

        //temp = GameObject.FindGameObjectWithTag("aassdd").GetComponent<Text>();

        //temp.text = currentDrones.ToString();
    }

    public override void Execute()
    {
        base.Execute();

        swarmController = Instantiate(swarmMaster, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
        firing = true;
    }

    public override bool CanExecute()
    {

        return swarmController == null && !isAbilityOnCoolDown;
    }

    public override void OnCooldownEnd()
    {
        if (!firing)
        {
            if (currentDrones < maxNanoDrones)
            {
                currentDrones++;
                if (currentDrones < maxNanoDrones) cooldownTimer.PlayFromStart();
            }
            //temp.text = currentDrones.ToString();
            isAbilityOnCoolDown = false;
        }
    }
    public void OnSwarmEnd()
    {
        firing = false;
        if (currentDrones == 0)
        {
            Debug.Log("Cooldown start");
            isAbilityOnCoolDown = true;
        }
        Destroy(swarmController);
    }
    public override void OnLifetimeEnd()
    {
        //cooldownTimer.PlayFromStart();
    }
    public void UpdateCurrentDrones()
    {
        currentDrones--;

        //temp.text = currentDrones.ToString();
    }
}
