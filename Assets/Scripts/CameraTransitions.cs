using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using DG.Tweening;

public class CameraTransitions : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerView;
    [SerializeField] CinemachineVirtualCamera coreView;
    [SerializeField] CinemachineVirtualCamera winView;

    [SerializeField] CinemachineBrain cameraBrain;
    [SerializeField] Camera _camera;

    [Header("Gameover Settings")]
    [Header("Includes Camera Blending")]
    [SerializeField] float timeTillExplosion = 3.0f;
    [SerializeField] GameObject CoreExplosion;
    [SerializeField] GameObject CombatHud;
    [SerializeField] GameObject fireWorks;

    private void Start()
    {
        LevelManager.Instance.Core.OnDeath += HandleLoseTransition;

        // This is just a ghost camera that follows our player around in the back. 
        playerView.Follow = LevelManager.Instance.Player.transform;
        playerView.LookAt = LevelManager.Instance.Player.transform;

        _camera.gameObject.SetActive(false);
    }

    private void HandleLoseTransition()
    {
        LevelManager.Instance.Player.playerInputs.SwitchCurrentActionMap("UI");
        LevelManager.Instance.Player.PlayerCam.gameObject.SetActive(false);

        CombatHud.SetActive(false);

        _camera.gameObject.SetActive(true);
        coreView.Priority = 15;
        playerView.Priority = 0;
        StartCoroutine(DisplayGameOverLose());
    }

    private IEnumerator DisplayGameOverLose()
    {
        yield return new WaitForSeconds(timeTillExplosion);
        CoreExplosion.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(CoreExplosion.transform.DOPunchScale(Vector3.one * 35, 2.5f, 0));
        sequence.Append(CoreExplosion.transform.DOScale(Vector3.one * 50.0f, 0.5f));

        sequence.Play().OnComplete(()=> LevelManager.Instance.GameOver(false));        
    }

    public void HandleWinTransition()
    {
        LevelManager.Instance.Player.playerInputs.SwitchCurrentActionMap("UI");
        LevelManager.Instance.Player.PlayerCam.gameObject.SetActive(false);



        CombatHud.SetActive(false);
        _camera.gameObject.SetActive(true);

        winView.Priority = 15;

        StartCoroutine(DisplayGameOverWin());
    }

    private IEnumerator DisplayGameOverWin()
    {
        yield return new WaitForSeconds(3.5f);
        fireWorks.SetActive(true);
        yield return new WaitForSeconds(6f);
        LevelManager.Instance.GameOver(true);
    }
}
