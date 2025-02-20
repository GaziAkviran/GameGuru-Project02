using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    static readonly int IsRunning = Animator.StringToHash("isRunning");
    static readonly int IsDancing = Animator.StringToHash("isDancing");
    static readonly int IsIdle = Animator.StringToHash("isIdle");
    [SerializeField, Foldout("References")] private Animator animator;

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
}
