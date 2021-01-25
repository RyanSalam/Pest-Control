using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AltFireAttachment : ScriptableObject
{
    protected Weapon weapon;

    [Space(10)]
    [Header("Alt Fire Stats")]
    [SerializeField] protected int energyCostPerFire = 50;
    [SerializeField] protected int secondaryDamage = 8;

    public abstract void AltShoot();

    public void initialize(Weapon weapon)
    {
        this.weapon = weapon;
    }
}

[CreateAssetMenu(fileName = "New ProjectileAttachment", menuName = "Attachment/Projectile")]
public class ProjectileAttachment : AltFireAttachment
{
    [SerializeField] protected Rigidbody projectilePrefab;
    public override void AltShoot()
    {
        Instantiate(projectilePrefab, weapon.FirePoint.position, weapon.FirePoint.rotation);
    }
}

[CreateAssetMenu(fileName = "New RayCastAttachment", menuName = "Attachment/RayCast")]
public class RayCastAttachment : AltFireAttachment
{
    int shotgunPellets = 8;
    int Damage = 2;
    int range = 10;

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
                        damageAmount = Damage,
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


