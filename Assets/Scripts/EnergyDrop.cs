using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EnergyDrop : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10.0f;
    [SerializeField] private int energyValue = 30;
    private Timer lifeTimer;

    private void Awake()
    {
        lifeTimer = new Timer(lifeTime);
        lifeTimer.OnTimerEnd += Despawn;
    }

    private void Update()
    {
        lifeTimer.Tick(Time.deltaTime);
    }

    private void Despawn()
    {
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        transform.DOScale(Vector3.one * 1.5f, 0.5f).From(Vector3.zero);
        lifeTimer.PlayFromStart();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.CurrentEnergy += energyValue;
            gameObject.SetActive(false);
        }
    }
}
