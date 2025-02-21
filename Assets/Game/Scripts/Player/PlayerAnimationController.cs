using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    static readonly int IsRunning = Animator.StringToHash("isRunning");
    static readonly int IsDancing = Animator.StringToHash("isDancing");
    static readonly int IsIdle = Animator.StringToHash("isIdle");
    
    [SerializeField, Foldout("References")] private Animator animator;
    [SerializeField, Foldout("References")] private Transform cameraRotateTransform;

    [Button()]
    public void PlayIdleAnimation()
    {
        animator.SetTrigger(IsIdle);
    }

    [Button()]
    public void PlayRunAnimation()
    {
        animator.SetTrigger(IsRunning);
    }

    [Button()]
    public void PlayDanceAnimation()
    {
        animator.SetTrigger(IsDancing);
    }

    public void RotateCameraTransform()
    {
        cameraRotateTransform.DOLocalRotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}
