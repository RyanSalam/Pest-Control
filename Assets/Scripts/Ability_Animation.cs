using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Ability", menuName = "Abilities/Animation Ability")]
public class Ability_Animation : Ability
{
    private Actor_Player pA;
    [SerializeField] private string triggerName;
    public float duration = 5f;
    [SerializeField] GameObject abilityInstance;
    GameObject abilityController;

    public override void Execute()
    {
        base.Execute();
        pA.Anim.SetTrigger(triggerName);
    }

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        abilityController = Instantiate(abilityInstance, player.AbilitySpawnPoint.position, player.AbilitySpawnPoint.rotation);
        abilityController.transform.SetParent(player.AbilitySpawnPoint);
        SwitchController(false);
    }

    public void SwitchController(bool isActivated)
    {
        abilityController.SetActive(isActivated);
    }

    public void DoCryoExplosion()
    {
        abilityController.GetComponent<Ability_Cryo>().CryostasisExplosion();
    }

    public override void OnCooldownEnd()
    {
        isAbilityOnCoolDown = false;
    }

    public override void OnLifetimeEnd()
    {
        isAbilityOnCoolDown = true;
    }

}
