using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class Traps_AnimationEvents : MonoBehaviour
{
    public GameObject trapParent;
    //public float dissolveTimer = 0.5f;
    //public SkinnedMeshRenderer[] trapBody;
    public GameObject trapDeathVFX;
    public float shrinkTimer = 2f; 

    public void OnEnable()
    {
        /*
        foreach (SkinnedMeshRenderer dissolveMat in trapBody)
        {
            dissolveMat.material.SetFloat("_TextureTransition", 0f); 
        }
        */
    }

    private void Start()
    {
        enabled = false;
        ObjectPooler.Instance.InitializePool(trapDeathVFX, 2);
    }

    public void Despawn()
    {
        LevelManager.Instance.AssessTraps(trapParent.GetComponent<Trap>());
        trapParent.SetActive(false);
        //StartCoroutine(Dissolve()); 
    }

    public void spawnVFX()
    {
        ObjectPooler.Instance.GetFromPool(trapDeathVFX, transform.position, trapDeathVFX.transform.rotation);
    }

    public void Shrink()
    {
        Debug.Log("i am shrinking"); 
        transform.root.DOScale(Vector3.zero, shrinkTimer).SetEase(Ease.InCirc);
    }
    /*
    IEnumerator Shrink()
    {
        float elapsed = 0;
        elapsed += Time.deltaTime;
        while (elapsed < shrinkTimer)
        {
            transform.root.localScale -= (Vector3.one/2) * Time.deltaTime;
            yield return null; 
        }
        transform.root.localScale = Vector3.one; 
    }
    */



    /*
    IEnumerator Dissolve()
    {
        float elapsed = 0;
        while (elapsed < dissolveTimer)
        {
            foreach (SkinnedMeshRenderer dissolve in trapBody)
            {
                Debug.Log("its desolving"); 
                elapsed += Time.deltaTime;
                float ratio = elapsed / dissolveTimer;
                ratio *= 2f;
                dissolve.material.SetFloat("_TextureTransition", ratio);
                yield return null;
            }
        }
        
    }
    */
}
