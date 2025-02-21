using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

public class CameraShakeController : MonoSingleton<CameraShakeController>
{
    [SerializeField, BoxGroup("Settings")] private float intensity;
    [SerializeField, BoxGroup("Settings")] private float duration;
    [SerializeField, Foldout("References")] private CinemachineVirtualCamera gameplayCamera;

    private CinemachineBasicMultiChannelPerlin perlin;

    void Start()
    {
        perlin = gameplayCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());

    }
    
    private IEnumerator ShakeRoutine()
    {
        perlin.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        perlin.m_AmplitudeGain = 0f;
    }
}
