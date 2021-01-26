using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttachment : MonoBehaviour
{
    //how many pellets we want to shoot out
    int shotgunPellets = 8;

    //range of our shotgun
    float range = 10f;

    int Damage = 1;

   
    public void FireShotgun(Vector3 FirePoint)
    {
        Debug.Log("Firing shotgun");
        //loop as many times as we want to shoot - pellet count
        for (int i = 0; i < shotgunPellets; i++)
        {
           //need two random variables to fluxuate our x/y - should simulate a shotgun spread.
            float randomX = Random.Range(-1.0f, 1.0f);
            float randomY = Random.Range(-1.0f, 1.0f);

            //assign them to our firePoint
            FirePoint.x += randomX;
            FirePoint.y += randomY;

            //now do our raycast with new coordinates
            if (Physics.Raycast(FirePoint,transform.forward,out RaycastHit hit, range))
            {
                if (hit.transform.CompareTag("Enemy")) //if we hit enemy -> damage it
                {
                    Actor enemyHit = hit.transform.GetComponent<Actor>();

                    //create our damageData struct, things we need to hurt enemies
                    DamageData damageData = new DamageData
                    {
                        //damager = player,
                        damageAmount = Damage,
                        direction = transform.forward,
                        damageSource = transform.position,
                        damagedActor = enemyHit,
                    };
                    //apply damage to our enemy
                    enemyHit.TakeDamage(damageData);

                }
            }
        }  
    }
}
