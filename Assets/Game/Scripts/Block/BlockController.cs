using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class BlockController : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("Settings")] private float moveDuration;
    
    [SerializeField, Foldout("References")] private MeshRenderer meshRenderer;

    private bool isMoving;
    private Tween moveTween;
    private SpawnSide spawnSide;

    public void Init(InitData data)
    {
        Reset();
        AssignRandomMaterial();

        if (data is BlockInitData blockData)
        {
            spawnSide = blockData.Side;
        }
        
        MoveBlock();
    }

    public void Reset()
    {
        transform.DOKill();
        isMoving = true;
    }

    private void AssignRandomMaterial()
    {
        meshRenderer.material = BlockColorManager.Instance.GetRandomMaterial();
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
        moveTween.Kill();
        BlockManager.Instance.OnBlockPlaced(this); 
    }
}
