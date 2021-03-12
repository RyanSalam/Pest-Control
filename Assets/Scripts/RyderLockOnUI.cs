using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RyderLockOnUI : MonoBehaviour
{
    private GameObject target;
    private bool isLockedOn;
    private Image lockOnImage;

    [SerializeField] Color defaultColor;
    [SerializeField] Color lockOnColor;

    [SerializeField] Vector3 lockOnScale;

    private void Awake()
    {
        lockOnImage = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        transform.LookAt(LevelManager.Instance.Player.PlayerCam.transform);
        transform.Rotate(0, 180, 0);
    }

    private void OnEnable()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DORotate(Vector3.up * 360, 0.2f));
        sequence.Join(lockOnImage.DOColor(lockOnColor, 0.25f).From(defaultColor));

        sequence.Play();
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
