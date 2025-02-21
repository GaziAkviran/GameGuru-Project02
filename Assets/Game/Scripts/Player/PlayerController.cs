using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, BoxGroup("Movement Settings")] private float forwardSpeed = 5f;
    [SerializeField, BoxGroup("Movement Settings")] private float horizontalSpeed = 5f;
    [SerializeField, BoxGroup("Movement Settings")] private float horizontalLimit = 5f;
    [SerializeField, BoxGroup("Movement Settings")] private float danceTransformMoveDuration = 1.5f;
    
    [SerializeField, Foldout("References")] private PlayerAnimationController animationController;
    [SerializeField, Foldout("References")] private Transform danceTransform;
    
    private Vector3 moveDirection;
    private float horizontalInput;
    private bool isGameRunning = false;
    
    private GameStateManager gameStateManager;

    private void Start()
    {
        gameStateManager = GameStateManager.Instance;
        gameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Update()
    {
        if (gameStateManager.CurrentState != GameState.Gameplay) return;
        CalculateMovement();
    }

    void FixedUpdate()
    {
        if (gameStateManager.CurrentState != GameState.Gameplay) return;
       
        MovePlayer();
    }
    
    private void CalculateMovement()
    {
        moveDirection = new Vector3(horizontalInput * horizontalSpeed, 0, forwardSpeed);
    }

    private void MovePlayer()
    {
        transform.Translate(moveDirection * Time.fixedDeltaTime);
    }
    
    private void OnDestroy()
    {
        if (gameStateManager != null)
        {
            gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
    
    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                StopMovement();
                animationController.PlayIdleAnimation();
                break;
                
            case GameState.Gameplay:
                StartMovement();
                animationController.PlayRunAnimation();
                break;
            
            case GameState.FinishRun:
                StopMovement();
                MoveDanceToTransform();
                animationController.PlayRunAnimation();
                break;
                
            case GameState.Win:
                StopMovement();
                animationController.RotateCameraTransform();
                animationController.PlayDanceAnimation();
                break;
                
            case GameState.Lose:
                StopMovement();
                animationController.PlayIdleAnimation();
                break;
        }
    }

    private void MoveDanceToTransform()
    {
        transform.DOMove(danceTransform.position, danceTransformMoveDuration)
            .OnComplete(() =>
            {
                GameStateManager.Instance.WinGame();
            });
    }
    
    private void StartMovement()
    {
        isGameRunning = true;
    }
    
    private void StopMovement()
    {
        isGameRunning = false;
        moveDirection = Vector3.zero;
    }
}
