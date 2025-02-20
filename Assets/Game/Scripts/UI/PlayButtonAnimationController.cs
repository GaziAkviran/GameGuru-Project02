using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
public class PlayButtonAnimationController : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float minScale;
    [SerializeField, BoxGroup("Settings")] private float maxScale;
    [SerializeField, BoxGroup("Settings")] private float duration;
    
    [SerializeField, Foldout("References")]private RectTransform rectTransform;
    
    private Sequence scaleSequence;

    private void Start()
    {
       PlayPulseAnimation();
    }

    [Button()]
    private void PlayPulseAnimation()
    {
        if (scaleSequence != null) 
            scaleSequence.Kill();
        
        scaleSequence = DOTween.Sequence();
        
        scaleSequence.Append(rectTransform.DOScale(Vector3.one * maxScale, duration)
            .SetEase(Ease.InOutQuad));
        
        scaleSequence.Append(rectTransform.DOScale(Vector3.one * minScale, duration)
            .SetEase(Ease.InOutQuad));
        
        scaleSequence.SetLoops(-1, LoopType.Yoyo);
    }
}
