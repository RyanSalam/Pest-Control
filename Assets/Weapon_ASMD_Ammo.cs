using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_ASMD_Ammo : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] int damage = 1;
    [SerializeField] float projForce = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * projForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("Enemy"))
        {
            Debug.Log("hit an enemy");
            Actor_Enemy enemy = c.gameObject.GetComponent<Actor_Enemy>();

            //damage enemy if it exists
            if (enemy != null)
            {
                DamageData damageData = new DamageData
                {
                    damager = LevelManager.Instance.Player,
                    damageAmount = damage,
                    direction = transform.position,
                    damagedActor = enemy
                };
                    enemy.TakeDamage(damageData);
            }
        }
        Destroy(gameObject);
    }


}
