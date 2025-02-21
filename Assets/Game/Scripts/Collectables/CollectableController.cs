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
    [SerializeField, BoxGroup("Idle Animation Settings")] private float minRotateDuration;
    [SerializeField, BoxGroup("Idle Animation Settings")] private float maxRotateDuration;
    
    [SerializeField, Foldout("References")] private Transform visual;
    [SerializeField, Foldout("References")] private ParticleSystem collectParticle;

    private Tween idleTween, collectTween, rotateTween;
    
    void Start()
    {
        PlayIdleAnimation();
        PlayRotateAnimation();
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
        rotateTween.Kill();
        
        collectTween = visual.DOScale(Vector3.zero, collectDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }

    private void PlayRotateAnimation()
    {
        var randomDuration = Random.Range(minRotateDuration, maxRotateDuration);
        rotateTween = transform.DOLocalRotate(new Vector3(0, 360, 0), randomDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    
    
}
