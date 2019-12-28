using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBoost : MonoBehaviour
{
    private Player _player;
    private bool _boosting;

    [FormerlySerializedAs("_cameraVaraibles")]
    public CameraManager cameraManager;
    
    
    public static Action<bool> boostEvent;
    private Coroutine _regenRoutine;
    private Coroutine _boostRoutine;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public void Boost(bool state)
    {
        if (!state && !_boosting) return;

        if (state && _player.remainingBoost > 0)
        {
            _boosting = true;
            cameraManager.ImpulseSource.GenerateImpulse();
            SetBoostSpeed(true);

            if (_boostRoutine == null)
                _boostRoutine = StartCoroutine(Boosting());

            return;
        }

        _boosting = false;
        SetBoostSpeed(false);
    }

    private void SetBoostSpeed(bool value)
    {
        if (value)
        {
            var speed = _player.forwardSpeed * 2;
            const float zoom = -7;
            cameraManager.SetCameraZoom(zoom, .4f);
            DOVirtual.Float(cameraManager.dolly.m_Speed, speed, .15f, cameraManager.SetSpeed);
            boostEvent?.Invoke(true);
        }
        else
        {
            var speed = _player.forwardSpeed;
            const float zoom = 0;
            cameraManager.SetCameraZoom(zoom, .4f);
            DOVirtual.Float(cameraManager.dolly.m_Speed, speed, .15f, cameraManager.SetSpeed);
            boostEvent?.Invoke(false);
            _regenRoutine = StartCoroutine(BoostRegen());
        }
    }

    private IEnumerator Boosting()
    {
        var waitTime = new WaitForSeconds(0.1f);
        
        while (_boosting)
        {
            _player.remainingBoost -= 0.1f;
            if (_player.remainingBoost <= 0 || Input.GetKeyUp(KeyCode.Space)) _boosting = false;
            yield return waitTime;
        }
        
        SetBoostSpeed(false);
        _boostRoutine = null;
        yield return null;
    }

    private IEnumerator BoostRegen()
    {
        var waitTime = new WaitForSeconds(0.1f);

        while (!_boosting && _player.remainingBoost < _player.maxBoost)
        {
            _player.remainingBoost += 0.1f;
            yield return waitTime;
        }
        
        _regenRoutine = null;
    }
}