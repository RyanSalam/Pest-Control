using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    bool isExecuting = false;


    //[SerializeField] Text temp;

    public int currentDrones;

    private Actor_Player player;

    public GameObject swarmMaster;

    SwarmSpawn swarmController;

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        player = abilitySource.GetComponent<Actor_Player>();
        currentDrones = maxNanoDrones;
        swarmController = Instantiate(swarmMaster, player.AbilitySpawnPoint.position, player.AbilitySpawnPoint.rotation).GetComponent<SwarmSpawn>();
        swarmController.transform.SetParent(player.AbilitySpawnPoint);
    }

    public override void Execute()
    {
        swarmController.gameObject.SetActive(true);
        isExecuting = true;
    }

    public override void HandleInput(InputAction.CallbackContext context)
    {
        if (!CanExecute()) return;

        if (context.action.name == AbilityButton)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Execute();
            }

            else if (context.phase == InputActionPhase.Canceled && swarmController.gameObject.activeSelf)
            {
                swarmController.ExitState();
                isExecuting = false;
                isAbilityOnCoolDown = true;
                cooldownTimer.PlayFromStart();
            }
        }
    }

    public override bool CanExecute()
    {
        return base.CanExecute();
    }

    public override void OnCooldownEnd()
    {
        isAbilityOnCoolDown = false;
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
