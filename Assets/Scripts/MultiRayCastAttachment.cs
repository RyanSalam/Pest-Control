using UnityEngine;

[CreateAssetMenu( menuName = "Attachment/Multi-RayCast")]
public class MultiRayCastAttachment : AltFireAttachment
{
    [SerializeField] int shotgunPellets = 8;

    [SerializeField] int range = 10;

    [SerializeField] int spreadAngle = 10;

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject impactEffect;

   

    
    public override void AltShoot()
    {

       
        //cache our weapons firepoint so we can make adjustments
        Vector3 firePos = weapon.FirePoint.position;

        Vector3 fireDirection;

        //Debug.Log("Firing shotgun");

        //vfx- muzzle flash
        GameObject tempVFX = Instantiate(muzzleFlash, firePos, weapon.FirePoint.rotation);
        Destroy(tempVFX, 0.15f);

        //loop as many times as we want to shoot - pellet count
        for (int i = 0; i < shotgunPellets; i++)
        {
            int randAnglePitch = Random.Range(-spreadAngle, spreadAngle);
            int randAngleYaw = Random.Range(-spreadAngle, spreadAngle);
            

            //Debug.Log("randAngle: " + randAnglePitch);

            Quaternion spreadAxis = Quaternion.AngleAxis(randAnglePitch, Vector3.right) * Quaternion.AngleAxis(randAngleYaw, Vector3.up);
            

            fireDirection = spreadAxis * weapon.FirePoint.forward;
          

            //now do our raycast with new coordinates
            if (Physics.Raycast(weapon.FirePoint.position, fireDirection, out RaycastHit hit, range))
            {
               
                if (hit.transform.CompareTag("Enemy")) //if we hit enemy -> damage it
                {
                    Actor enemyHit = hit.transform.GetComponent<Actor>();

                    //create our damageData struct, things we need to hurt enemies
                    DamageData damageData = new DamageData
                    {
                        //damager = player,
                        damageAmount = secondaryDamage,
                        direction = weapon.FirePoint.position,
                        damageSource = hit.point,
                        damagedActor = enemyHit,
                        hitNormal = hit.normal,
                    };

                    if (enemyHit != null)
                    {
                        //apply damage to our enemy
                        enemyHit.TakeDamage(damageData);
                        ImpactSystem.Instance.DamageIndication(damageData.damageAmount, Color.blue , damageData.damageSource, Quaternion.LookRotation(-hit.normal));
                    }

                    //ImpactSystem.Instance.HandleImpact
                }

                if (impactEffect != null)
                {
                    GameObject impactVFX = impactEffect;
                    impactVFX.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                    impactVFX = Instantiate(impactVFX, hit.point, Quaternion.LookRotation(hit.point));
                    Destroy(impactVFX, 0.15f);
                }

                ImpactSystem.Instance.HandleImpact(hit.transform.gameObject, hit.point, Quaternion.LookRotation(hit.normal), Color.blue);

            }
        }
    }
    
}


