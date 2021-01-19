using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsBuildable;
    [SerializeField] protected int trapDamage = 1;
    [SerializeField] protected int maxUses;
    [HideInInspector] protected float movementSpeed;

    public virtual bool Buildable()
    {
        Collider[] objects = Physics.OverlapBox(transform.position, Vector3.one * 0.4f);

        foreach (Collider col in objects)
        {
            if (col.GetComponent<Actor>() == true && col != this.GetComponent<Collider>())
            {
                return false;
            }
        }

        return true;
    }
    public void Build()
    {
        enabled = true;
        transform.parent = null;
        GetComponent<AudioCue>().PlayAudioCue(); //for audio
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
        Gizmos.color = Color.red;
    }

    public virtual void Interact(Actor_Player player)
    {
        //player energy refund here
    }
}
