using UnityEngine;

[CreateAssetMenu( menuName = "Attachment/Projectile")]
public class ProjectileAttachment : AltFireAttachment
{
    [SerializeField] protected Rigidbody projectilePrefab;
    public override void AltShoot()
    {
        Instantiate(projectilePrefab, weapon.FirePoint.position, weapon.FirePoint.rotation);
    }
}


