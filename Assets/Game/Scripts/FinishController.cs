using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    [SerializeField, Foldout("References")] private List<ParticleSystem> confettiParticles;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayConfettiParticles();
            GameStateManager.Instance.FinishRun();
        }
    }

    private void PlayConfettiParticles()
    {
        foreach (var confettiParticle in confettiParticles)
        {
            confettiParticle.Play();
        }
    }
}
