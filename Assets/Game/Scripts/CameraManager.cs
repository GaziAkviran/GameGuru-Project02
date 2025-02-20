using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
     [System.Serializable]
    public class GameStateCameraPair
    {
        public GameState gameState;
        public CinemachineVirtualCamera virtualCamera;
    }

    [SerializeField] private GameStateCameraPair[] stateVirtualCameras;
    
    private CinemachineVirtualCamera currentCamera;
    private GameStateManager gameStateManager;
    private Dictionary<GameState, CinemachineVirtualCamera> cameraDict = new Dictionary<GameState, CinemachineVirtualCamera>();
    
    protected override void Awake()
    {
        base.Awake();
        
        foreach (GameStateCameraPair pair in stateVirtualCameras)
        {
            if (pair.virtualCamera != null)
            {
                cameraDict[pair.gameState] = pair.virtualCamera;
                pair.virtualCamera.Priority = 0;
            }
        }
    }
    
    private void Start()
    {
        gameStateManager = GameStateManager.Instance;
        
        if (gameStateManager != null)
        {
            gameStateManager.OnGameStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(gameStateManager.CurrentState);
        }
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
        if (cameraDict.TryGetValue(newState, out CinemachineVirtualCamera camera))
        {
            SwitchToCamera(camera);
        }
    }
    
    private void SwitchToCamera(CinemachineVirtualCamera camera)
    {
        if (camera == null) return;
        
        foreach (GameStateCameraPair pair in stateVirtualCameras)
        {
            if (pair.virtualCamera != null)
            {
                pair.virtualCamera.Priority = (pair.virtualCamera == camera) ? 20 : 10;
            }
        }
        
        currentCamera = camera;
    }
    
    public void SwitchToCamera(GameState gameState)
    {
        if (cameraDict.TryGetValue(gameState, out CinemachineVirtualCamera camera))
        {
            SwitchToCamera(camera);
        }
    }
}
