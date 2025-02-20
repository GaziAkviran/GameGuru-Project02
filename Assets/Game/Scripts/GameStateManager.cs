using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
    Win,
    Lose
}

public class GameStateManager : MonoSingleton<GameStateManager>
{
    private GameState currentState;
    
    public event Action<GameState> OnGameStateChanged;
    
    public GameState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
    
    public void StartGame()
    {
        if (currentState == GameState.MainMenu)
        {
            ChangeState(GameState.Gameplay);
        }
    }
    
    public void WinGame()
    {
        if (currentState == GameState.Gameplay)
        {
            ChangeState(GameState.Win);
        }
    }
    
    public void LoseGame()
    {
        if (currentState == GameState.Gameplay)
        {
            ChangeState(GameState.Lose);
        }
    }
    
    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }
}
