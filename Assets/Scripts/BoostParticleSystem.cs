using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class BoostParticleSystem : MonoBehaviour
{
    [Space] [Header("Particles")] public ParticleSystem trail;

    public ParticleSystem circle;
    public ParticleSystem barrel;

    [FormerlySerializedAs("_trailRenderer")]
    public TrailRenderer trailRenderer;

    public Transform cameraParent;
    public ParticleSystem stars;
    
    private void Awake()
    {
        PlayerMovement.BarrelRoll += barrel.Play;
        
        PlayerMovement.BoostEvent += Boost;
        Player.OnDeath += () => Boost(false);
    }

    private void Boost(bool value)
    {
        if (value)
        {
            trail.Play();
            circle.Play();
            BoostEffects(true);
            return;
        }
        
        trail.Stop();
        circle.Stop();
        BoostEffects(false);
    }

    private void BoostEffects(bool state)
    {
        var origFov = state ? 40 : 55;
        var endFov = state ? 55 : 40;
        var origChrom = state ? 0 : 1;
        var endChrom = state ? 1 : 0;
        var origDistortion = state ? 0 : -30;
        var endDistorton = state ? -30 : 0;

        DOVirtual.Float(origChrom, endChrom, .5f, Chromatic);
        DOVirtual.Float(origFov, endFov, .5f, FieldOfView);
        DOVirtual.Float(origDistortion, endDistorton, .5f, DistortionAmount);
        
        var starsVel = state ? -20 : -1;
        var pvel = stars.velocityOverLifetime;
        pvel.z = starsVel;

        trailRenderer.emitting = state;
    }

    private void DistortionAmount(float x)
    {
        if (Camera.main != null)
            Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().intensity.value = x;
    }

    private void FieldOfView(float fov)
    {
        cameraParent.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
    }

    private void Chromatic(float x)
    {
        if (Camera.main != null)
            Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().intensity.value = x;
    }
}