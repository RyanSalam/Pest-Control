using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimEvent : MonoBehaviour
{
    Actor_Enemy thisEnemy;
    int damage;


    public float dissolveTime;
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer wings;

    public void OnEnable()
    {
        body.material.SetFloat("_TextureTransition", 0f);
        wings.material.SetFloat("_TextureTransition", 0f);
    }

    private void Start()
    {
        // Assign the enemy reference accordingly.
        thisEnemy = GetComponentInParent<Actor_Enemy>();
        damage = (int)thisEnemy.Damage;
        enabled = false;
    }

    public void DoAttack()
    {
        Collider[] targets = Physics.OverlapBox(transform.position, Vector3.one * thisEnemy.AttackRange, thisEnemy.transform.rotation);

        foreach (Collider col in targets)
        {
            if (!col.CompareTag("Enemy"))
            {
                Actor actor = col.GetComponent<Actor>();

                if (actor != null)
                {
                    DamageData data = new DamageData()
                    {
                        damageAmount = damage,
                        damager = thisEnemy,
                        damagedActor = actor,
                        direction = transform.forward,
                        damageSource = transform.position
                    };

                    actor.TakeDamage(data);
                    break;
                }
            }
        }
    }

    public void Ragdoll()
    {

    }

    public void Despawn()
    {
        StartCoroutine(Dissolve());
    }

    IEnumerator Dissolve()
    {
        float elapsed = 0;
        while(elapsed < dissolveTime)
        {
            Debug.Log("Despawn");
            elapsed += Time.deltaTime;
            float ratio = elapsed / dissolveTime;
            ratio *= 0.7f;
            body.material.SetFloat("_TextureTransition", ratio);
            wings.material.SetFloat("_TextureTransition", ratio);
            yield return null;
        }
        thisEnemy.gameObject.SetActive(false);
    }
}
