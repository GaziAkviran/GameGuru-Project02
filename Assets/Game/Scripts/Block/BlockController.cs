using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class BlockController : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("Settings")] private float moveDuration;
    
    [SerializeField, Foldout("References")] private MeshRenderer mainMeshRenderer;
    [SerializeField, Foldout("References")] private Transform mainCube;
    [SerializeField, Foldout("References")] private Transform cutCube;
    [SerializeField, Foldout("References")] private BlockCutter blockCutter; 
    
    private bool isMoving;
    private Tween moveTween;
    private SpawnSide spawnSide;
    private Material blockMaterial;

    private BlockController lastBlock;

    public Transform GetMainBlock() => mainCube;
    public Transform GetCutPiece() => cutCube;

    public void Init(InitData data)
    {
        Reset();
        AssignRandomMaterial();

        if (data is BlockInitData blockData)
        {
            spawnSide = blockData.Side;
        }
    
        cutCube.gameObject.SetActive(false);
        cutCube.localScale = Vector3.zero;
        
        MoveBlock();
    }

    public void Reset()
    {
        transform.DOKill();
        isMoving = true;
        
        cutCube.gameObject.SetActive(false);
        cutCube.localScale = Vector3.zero;
    }

    private void AssignRandomMaterial()
    {
        blockMaterial = BlockColorManager.Instance.GetRandomMaterial();
        mainMeshRenderer.material = blockMaterial;
    
        MeshRenderer cutRenderer = cutCube.GetComponent<MeshRenderer>();
        if (cutRenderer != null)
        {
            cutRenderer.material = blockMaterial;
        }
    }
    
    private void MoveBlock()
    {
        var leftTarget = BlockManager.Instance.LeftPoint;
        var rightTarget = BlockManager.Instance.RightPoint;
    
        if (leftTarget == null || rightTarget == null) return;

        float targetX = (spawnSide == SpawnSide.Left) ? rightTarget.position.x : leftTarget.position.x;

        moveTween = transform.DOMoveX(targetX, moveDuration)
            .SetEase(Ease.Linear);
    }

    public void StopBlock()
    {
        if (!isMoving) return;

        isMoving = false;

        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
        
        BlockManager.Instance.OnBlockPlaced(this);
        CheckAndCutBlock();
    }

    private void CheckAndCutBlock()
    {
        if (BlockManager.Instance.ActiveBlocks.Count < 2) return;

        lastBlock = BlockManager.Instance.ActiveBlocks[BlockManager.Instance.ActiveBlocks.Count - 3];
        
        if (blockCutter != null)
        {
            bool isCut = blockCutter.TryCutBlock(this, lastBlock);
        }
    }
}