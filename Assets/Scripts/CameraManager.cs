using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public Transform cameraParent;
    public CinemachineDollyCart dolly;
    public CinemachineImpulseSource ImpulseSource { get; private set; }

    private void Awake()
    {
        ImpulseSource = cameraParent.GetComponentInChildren<CinemachineImpulseSource>();
    }

    public void SetCameraZoom(float zoom, float duration) =>
        cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    
    public void SetSpeed(float x) => dolly.m_Speed = x;
    
}