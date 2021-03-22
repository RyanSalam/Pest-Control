﻿using System.Collections;
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
    [SerializeField] GameObject damageIndicatorObj;

    public Dictionary<Material, ImpactSettings> ImpactDictionary;
    [SerializeField] ImpactSettings defaultSettings;
    public AudioCue ac;

    [SerializeField] AudioCue uiAudio;

    protected override void Awake()
    {
        base.Awake();
        ac = GetComponent<AudioCue>();
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

   

    public void HandleImpact(GameObject targetHit, Vector3 hitPoint, Quaternion hitRotation, Color weaponColour)
    {
        MeshRenderer myMeshRenderer = targetHit.GetComponent<MeshRenderer>();
        
        Material myMaterial = null;

        if (myMeshRenderer != null)
        {
            myMaterial = myMeshRenderer.sharedMaterial;
        }
       

        if (myMaterial == null)
            return;

        ImpactSettings impactSettings;


        //if it doesnt exist gtfo
        if (ImpactDictionary.ContainsKey(myMaterial))
            impactSettings = ImpactDictionary[myMaterial];
        else
            impactSettings = defaultSettings;

        ObjectPooler.Instance.GetFromPool(impactSettings.decalToSpawn, hitPoint, hitRotation);
        ObjectPooler.Instance.GetFromPool(impactSettings.particleToSpawn, hitPoint, hitRotation);
        
        ac.PlayAudioCue(impactSettings.audioToPlay, 100);
        
    }

    public virtual void DamageIndication(float damage, Color color, Vector3 position, Quaternion rotation)
    {
        if (damageIndicatorObj != null)
        {
            GameObject temp = ObjectPooler.Instance.GetFromPool(damageIndicatorObj, position, rotation).gameObject;
            temp.GetComponent<DamageIndicator>().setDamageIndicator((int)damage, color);
        }
    }

    public void PlayUISoundSFX(AudioCueSO audioToPlay)
    {
        if (audioToPlay == null) return;
        uiAudio.PlayAudioCue(audioToPlay);
    }

}
