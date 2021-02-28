using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSystem : MonoSingleton<ImpactSystem>
{
    [System.Serializable]

   public class ImpactSettings
    {
        public Material hitMaterial;
        public GameObject decalToSpawn;
        public GameObject particleToSpawn;
        public AudioCueSO audioToPlay;
    }

    [SerializeField] ImpactSettings[] WorldImpactSetting;


    public Dictionary<Material, ImpactSettings> ImpactDictionary;
    [SerializeField] ImpactSettings defaultSettings;
    protected override void Awake()
    {
        base.Awake();

        //create a new dictionary on spawn
        ImpactDictionary = new Dictionary<Material, ImpactSettings>();

        //creatign the key for our dictionary, which is the material we hit.
        //Initializing object pools for each decal and particel to spawn based of what it hit
        foreach(ImpactSettings Setting in WorldImpactSetting)
        {
            ImpactDictionary[Setting.hitMaterial] = Setting;
            
            if(Setting.decalToSpawn != null)
               ObjectPooler.Instance.InitializePool(Setting.decalToSpawn, 20);

            if (Setting.particleToSpawn != null)
                ObjectPooler.Instance.InitializePool(Setting.particleToSpawn, 10);
        }

        ObjectPooler.Instance.InitializePool(defaultSettings.decalToSpawn, 20);
        ObjectPooler.Instance.InitializePool(defaultSettings.particleToSpawn, 10);
    }

    public void HandleImpact(GameObject targetHit, Vector3 hitPoint, Quaternion hitRotation)
    {
        //MeshRenderer myParentMeshRenderer = targetHit.GetComponent<MeshRenderer>();

        MeshRenderer myMeshRenderer = targetHit.GetComponent<MeshRenderer>();
        MeshRenderer[] myMeshRenderer1 = targetHit.GetComponentsInChildren<MeshRenderer>();

        Debug.Log(myMeshRenderer1.Length);
        //Material myMaterial = targetHit.GetComponent<MeshRenderer>().material;
        Material myMaterial = null;

        if (myMeshRenderer != null)
            myMaterial = myMeshRenderer.material;

        if (!myMeshRenderer)
        {
            float childCount = targetHit.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                GameObject temp = targetHit.transform.GetChild(i).gameObject;

                MeshRenderer meshRenderer = temp.GetComponent<MeshRenderer>();

                if (meshRenderer != null) //found a mesh renderer
                {
                    myMaterial = meshRenderer.material;
                    break;
                }
            }
        }

        if (myMaterial == null)
            return;


        ImpactSettings impactSettings;


        //if it doesnt exist gtfo
        if (!ImpactDictionary.ContainsKey(myMaterial))
            impactSettings = defaultSettings;
        else
            impactSettings = ImpactDictionary[myMaterial];


        ObjectPooler.Instance.GetFromPool(impactSettings.decalToSpawn, hitPoint, hitRotation);
        ObjectPooler.Instance.GetFromPool(impactSettings.particleToSpawn, hitPoint, hitRotation);
    }



}
