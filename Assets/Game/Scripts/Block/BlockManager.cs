using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum SpawnSide
{
    Left,
    Right
}

public class BlockManager : MonoSingleton<BlockManager>
{
    [SerializeField, BoxGroup("Settings")] private int poolSize = 20;
    [SerializeField, BoxGroup("Settings")] private float blockSize = 3f;
    [SerializeField, BoxGroup("Settings")] private float spawnInterval = 0.5f;
    
    [SerializeField, Foldout("References")] private BlockController blockPrefab;
    [SerializeField, Foldout("References")] private Transform leftPoint;
    [SerializeField, Foldout("References")] private Transform rightPoint;
    
    private ObjectPool<BlockController> blockPool;
    private List<BlockController> activeBlocks = new List<BlockController>();
    private bool isSpawning = false;
    private SpawnSide lastSpawnSide = SpawnSide.Right;
    private float currentZPosition = 0f;
    private GameStateManager gameStateManager;

    public Transform LeftPoint => leftPoint;
    public Transform RightPoint => rightPoint;
    
    private void Start()
    {
        gameStateManager = GameStateManager.Instance;

        if (gameStateManager != null)
        {
            gameStateManager.OnGameStateChanged += HandleGameStateChanged;
        }
        InitializeBlockPool();
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay) return;
        
        HandleBlockPlacement();
    }
    
    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                StartSpawning();
                break;

            case GameState.MainMenu:
            case GameState.Win:
            case GameState.Lose:
                StopSpawning();
                break;
        }
    }

    private void InitializeBlockPool()
    {
        if (blockPrefab != null)
        {
            blockPool = ObjectPoolManager.Instance.GetOrCreatePool(blockPrefab, poolSize, transform);
        }
        else
        {
            Debug.LogError("Block prefab is not assigned!");
        }
    }

    [Button()]
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            SpawnNextBlock();
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void SpawnNextBlock()
    {
        if (!isSpawning) return;

        BlockController newBlock = SpawnBlock();
        
        if (newBlock != null)
        {
            activeBlocks.Add(newBlock);
        }
    }

    public BlockController SpawnBlock()
    {
        if (blockPool == null) return null;

        SpawnSide side = GetNextSpawnSide();
        Transform spawnPoint = (side == SpawnSide.Left) ? leftPoint : rightPoint;

        float spawnZ = (activeBlocks.Count > 0) 
            ? activeBlocks[activeBlocks.Count - 1].transform.position.z + blockSize 
            : spawnPoint.position.z;

        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnZ);

        BlockController block = blockPool.Get(spawnPosition, spawnPoint.rotation, new BlockInitData { Side = side });

        if (block != null)
        {
            lastSpawnSide = side;
            return block;
        }

        return null;
    }


    private SpawnSide GetNextSpawnSide()
    {
        return (lastSpawnSide == SpawnSide.Left) ? SpawnSide.Right : SpawnSide.Left;
    }

    public void OnBlockPlaced(BlockController placedBlock)
    {
        SpawnNextBlock();
    }
    
    private void HandleBlockPlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeBlocks.Count > 0)
            {
                activeBlocks[activeBlocks.Count - 1].StopBlock();
            }
        }
    }

    private void OnDestroy()
    {
        if (gameStateManager != null)
        {
            gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

}
