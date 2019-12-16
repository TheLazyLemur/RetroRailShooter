using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovement : MonoBehaviour
{
    public static Action BarrelRoll;
    public static Action<bool> BoostEvent;

    private Transform _playerModel;
    public float forwardSpeed = 6;


    [Space] [Header("Public References")] public Transform aimTarget;
    public CinemachineDollyCart dolly;
    public Transform cameraParent;

    private Camera _camera;
    private CinemachineImpulseSource _impulseSource;
    private Player _player;


    private Coroutine _routine;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _camera = Camera.main;
        _impulseSource = cameraParent.GetComponentInChildren<CinemachineImpulseSource>();
        _playerModel = transform.GetChild(0);

        Player.OnDeath += () => Destroy(this);
    }

    private void Start()
    {
        _player._remainingBoost = _player._maxBoost;
        dolly.m_Speed = forwardSpeed;
    }


    public void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += Time.deltaTime * speed * new Vector3(x, y, 0);
        ClampPosition();
    }

    private void ClampPosition()
    {
        var pos = _camera.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = _camera.ViewportToWorldPoint(pos);
    }


    public void RotationLook(float h, float v, float speed)
    {
        aimTarget.parent.position = Vector3.zero;
        aimTarget.localPosition = new Vector3(h, v, 1);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimTarget.position),
            Mathf.Deg2Rad * speed * Time.deltaTime);
    }

    public void HorizontalLean(float axis, float leanLimit, float lerpTime)
    {
        var target = _playerModel;
        var targetEulerAngels = target.localEulerAngles;

        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y,
            Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }

    public void QuickSpin(int dir)
    {
        if (DOTween.IsTweening(_playerModel)) return;

        var localEulerAngles = _playerModel.localEulerAngles;
        _playerModel.DOLocalRotate(
            new Vector3(localEulerAngles.x, localEulerAngles.y, 360 * -dir), .4f,
            RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);

        BarrelRoll.Invoke();
    }

    private bool _boosting;
    
    private void SetSpeed(float x) => dolly.m_Speed = x;

    private void SetCameraZoom(float zoom, float duration) =>
        cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    

    public void Update()
    {
        if (_boosting || !(_player._remainingBoost < 100)) return;
        if (_player._remainingBoost >= _player._maxBoost) _player._remainingBoost = 100;
        
        _player._remainingBoost += Time.deltaTime * 20;
        if (_player._remainingBoost > _player._maxBoost) _player._remainingBoost = _player._maxBoost;
    }

    
    public void Boost(bool state)
    {
        if (state == false && _boosting == false) return;
        
        if (state && _player._remainingBoost > 0)
        {
            _boosting = true;
            _impulseSource.GenerateImpulse();
            SetBoostSpeed();
            
            if (_routine == null)
                _routine = StartCoroutine(Boosting());
            
            return;
        }

        _boosting = false;
        ResetBoostSpeed();
    }

    private void SetBoostSpeed()
    {
        const int speed = 12;
        const float zoom = -7;
        SetCameraZoom(zoom, .4f);
        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        BoostEvent?.Invoke(true);
    }

    private void ResetBoostSpeed()
    {
        const int speed = 6;
        const float zoom = 0;
        SetCameraZoom(zoom, .4f);
        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        BoostEvent?.Invoke(false);
    }


    private IEnumerator Boosting()
    {
        while (_boosting)
        {
            _player._remainingBoost -= 1f;
            if (_player._remainingBoost <= 0 || Input.GetKeyUp(KeyCode.Space)) _boosting = false;
            yield return new WaitForSeconds(0.0f);
        }

        ResetBoostSpeed();
        _routine = null;
        yield return null;
    }

    public void Break(bool state)
    {
        var speed = state ? forwardSpeed / 3 : forwardSpeed;
        var zoom = state ? 3 : 0;
        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        SetCameraZoom(zoom, .4f);
    }
}