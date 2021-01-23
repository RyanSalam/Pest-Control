using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Laser : Trap
{
    // Start is called before the first frame update
    [SerializeField] GameObject spikes;
    [SerializeField] bool active = false;
    IEnumerator ActivateSpikes()
    {
        active = true; // on collision with trigger
        spikes.transform.Translate(Vector3.up * 0.4f, spikes.transform); //when active, laser shoot up on Y axis
        yield return new WaitForSeconds(1.0f); //makes the trap wait for before activating again
        spikes.transform.Translate(Vector3.up * -0.4f, spikes.transform); //retract laser
        active = false; //reset
    }
    private void OnTriggerEnter(Collider trigger)
    {
        if (maxUses > 0) //checks max uses
        {
            if (trigger.gameObject.GetComponent<Actor_Enemy>()) //if the enemy actor collides with trap
            {
                StartCoroutine("ActivateLasers"); //intiate lasers
                // Adjusting the damageData based on information we have
                DamageData data = new DamageData //calling variables from struct and making a local variable
                {
                    damageAmount = trapDamage, //damage struct takes values from base trap script
                    direction = this.transform.up, //direction of laser location
                    damageSource = this.transform.position //where the trap is located
                };
                trigger.GetComponent<Actor_Enemy>().TakeDamage(data); //enemy actor, on trigger will take the new damage data

            }
            maxUses--; //decreases nax uses of trap 
        }
        else
        {
            Destroy(gameObject); // destroy trap at max use
        }
    }
}
