using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_ExpertASMD : Weapon
{
    //SHOCK RIFFLE FROM UNREAL TOURNAMENT - AKA - THE EXPERT ASMD FROM BORDERLANDS3 - YOU PLANNING A FOLLOW UP ?
    [SerializeField] Rigidbody projectilePrefab;
    protected Rigidbody tempProjectile;

    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] private ParticleSystem bulletVFX;


    public override void PrimaryFire()
    {
        //base.PrimaryFire();
        if (Time.time > fireRate + lastFired && canFire)
        {
            tempProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);


            base.PrimaryFire();
        }
    }

    private void OnEnable()
    {
        //need to switch our tiling offset for the weapon icon
        SkinnedMeshRenderer skinnedMesh = spriteScreen.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
            MaterialHandler.Vector2Changer(skinnedMesh, "_TilingOffset", new Vector2(1.4f, 0f));
    }
    public override void Release()
    {
        base.Release();
    }
}
