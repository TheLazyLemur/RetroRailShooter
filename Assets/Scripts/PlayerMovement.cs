using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [FormerlySerializedAs("_cameraVaraibles")] public CameraManager cameraManager;
    [Space] [Header("Public References")] public Transform aimTarget;

    public static Action BarrelRoll;
    
    private Transform _playerModel;
    private Camera _camera;
    private Player _player;

    private void Awake()
    {
        _camera = Camera.main;
        _player = GetComponent<Player>();
        _playerModel = transform.GetChild(0);
        Player.onDeath += () => Destroy(this);
    }

    private void Start()
    {
        _player.remainingBoost = _player.maxBoost;
        cameraManager.dolly.m_Speed = _player.forwardSpeed;
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

    public void Break(bool state)
    {
        var speed = state ? _player.forwardSpeed / 3 : _player.forwardSpeed;
        var zoom = state ? 3 : 0;
        DOVirtual.Float(cameraManager.dolly.m_Speed, speed, .15f, cameraManager.SetSpeed);
        cameraManager.SetCameraZoom(zoom, .4f);
    }
}