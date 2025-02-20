using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuPanel : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float fadeDuration;
    [SerializeField, Foldout("References")] private Button playButton;
    [SerializeField, Foldout("References")] private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
    }
    
    private void OnPlayButtonClicked()
    {
        GameStateManager.Instance.ChangeState(GameState.Gameplay);
        HideCanvas();
    }

    private void HideCanvas()
    {
        canvasGroup.DOFade(0, fadeDuration);
    }

    private void ShowCanvas()
    {
        canvasGroup.DOFade(1, fadeDuration);
    }

    
}
