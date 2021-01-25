using UnityEngine;

[CreateAssetMenu( menuName = "Attachment/Multi-RayCast")]
public class MultiRayCastAttachment : AltFireAttachment
{
    [SerializeField] int shotgunPellets = 8;

    [SerializeField] int range = 10;

    [SerializeField] int spreadAngle = 10;

    public override void AltShoot()
    {

       
        //cache our weapons firepoint so we can make adjustments
        Vector3 firePos = weapon.FirePoint.position;

        Vector3 fireDirection;

        Debug.Log("Firing shotgun");

        Debug.DrawRay(weapon.FirePoint.position, weapon.FirePoint.forward, Color.red);

        //loop as many times as we want to shoot - pellet count
        for (int i = 0; i < shotgunPellets; i++)
        {
            int randAnglePitch = Random.Range(-spreadAngle, spreadAngle);
            int randAngleYaw = Random.Range(-spreadAngle, spreadAngle);
            

            Debug.Log("randAngle: " + randAnglePitch);

            Quaternion spreadAxis = Quaternion.AngleAxis(randAnglePitch, Vector3.right) * Quaternion.AngleAxis(randAngleYaw, Vector3.up);
            

            fireDirection = spreadAxis * weapon.FirePoint.forward;
          

            Debug.DrawRay(weapon.FirePoint.position, fireDirection, Color.green);

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
                        damageSource = weapon.FirePoint.position,
                        damagedActor = enemyHit,
                    };
                    //apply damage to our enemy
                    enemyHit.TakeDamage(damageData);

                }
            }
        }
    }
    
}


