using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CollectableType
{
    Star,
    Gold,
    Diamond
}

public class CollectableController : MonoBehaviour
{
    [SerializeField, BoxGroup("Type Settings")] private CollectableType type;
    
    [SerializeField, BoxGroup("Collect Animation Settings")] private float collectDuration;
    
    [SerializeField, BoxGroup("Idle Animation Settings")] private float maxHeight;
    [SerializeField, BoxGroup("Idle Animation Settings")] private float minHeight;
    [SerializeField, BoxGroup("Idle Animation Settings")] private float minIdleDuration;
    [SerializeField, BoxGroup("Idle Animation Settings")] private float maxIdleDuration;
    
    [SerializeField, Foldout("References")] private Transform visual;
    [SerializeField, Foldout("References")] private ParticleSystem collectParticle;

    private Tween idleTween, collectTween;
    
    void Start()
    {
        PlayIdleAnimation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        collectParticle.Play();
        PlayCollectAnimation();
    }
    
    private void PlayIdleAnimation()
    {
        var randomDuration = Random.Range(minIdleDuration, maxIdleDuration);
        idleTween = visual.DOLocalMoveY(maxHeight, randomDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void PlayCollectAnimation()
    {
        idleTween.Kill();
        
        collectTween = visual.DOScale(Vector3.zero, collectDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
    
    
}
