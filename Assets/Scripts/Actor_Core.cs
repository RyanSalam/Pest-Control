using UnityEngine;
using DG.Tweening;

public class Actor_Core : Actor
{
    // Serializable class so it shows up in the inspector without needing to create manually 
    [System.Serializable]
    class CoreRing 
    {
        [Header("Default Set Up")]
        [SerializeField] Rigidbody ring;
        [SerializeField] Vector3 rotationDir;
        [SerializeField] float speed;

        [Header("Attributes For Ring Dropping")]
        [Tooltip("The health percentage required to turn this ring off. 1 is 100% (Full Health) and 0 is 0% (No Health) ")]
        [Range(0, 1)][SerializeField] float healthRequirement = 0.3f;
        [Tooltip("The rotation that this ring will interpolate back to and then drop")]
        [SerializeField] Vector3 restRotation;
        [Tooltip("The time it will take to interpolate to it's resting rotation")]
        [SerializeField] float interpolateDuration = 0.3f;

        private bool bIsRotating = true;

        public void HandleRingRotation()
        {
            if (!bIsRotating) return;
            ring.transform.Rotate(rotationDir * speed * Time.deltaTime);
        }

        // Delegate takes 
        public void CheckThreshold(float current, float max)
        {
            if (!bIsRotating) return;

            float percent = current / max;

            if (percent <= healthRequirement)
            {
                bIsRotating = false;
                ring.transform.DORotate(restRotation, interpolateDuration).OnComplete(() => 
                    ring.isKinematic = false);
                Actor_Player p = FindObjectOfType<Actor_Player>();
                p._audioCue.PlayAudioCue(p._cInfo.CoreDamaged);
            }
        }
    }

    [SerializeField] CoreRing[] rings;

    protected override void Start()
    {
        base.Start();

        // Binding the core's health change delegate to each ring's check function
        foreach (CoreRing ring in rings)
        {
            OnHealthChanged += ring.CheckThreshold;
            
        }
    }

    protected virtual void Update()
    {   // We handle the rotation for each ring class
        foreach(CoreRing ring in rings)
        {
            ring.HandleRingRotation();
        }
    }

    
}
