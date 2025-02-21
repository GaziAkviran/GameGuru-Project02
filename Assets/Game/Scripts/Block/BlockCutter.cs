using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class BlockCutter : MonoBehaviour
{
    [SerializeField, BoxGroup("Cut Settings")] private float minCutThreshold = 0.001f;
    [SerializeField, BoxGroup("Cut Settings")] private float minimumVisibleCut = 0.05f;
    [SerializeField, BoxGroup("Cut Settings")] private bool showDebugInfo = true;
    
    [SerializeField, BoxGroup("Falling Peaces Settings")] private float fallingPieceLifetime = 2f;
    [SerializeField, BoxGroup("Falling Peaces Settings")] private float fallingPieceMass = 0.5f;
    
    private bool IsPerfectCut(float newWidth, float lastWidth, float tolerance = 0.05f)
    {
        return Mathf.Abs(newWidth - lastWidth) <= tolerance;
    }
    
    public bool TryCutBlock(BlockController currentBlock, BlockController lastBlock)
    {
        if (!ValidateBlocks(currentBlock, lastBlock))
            return false;
  
        BlockData currentBlockData = GetBlockData(currentBlock);
        BlockData lastBlockData = GetBlockData(lastBlock);

        BlockBounds currentBounds = CalculateBlockBounds(currentBlockData);
        BlockBounds lastBounds = CalculateBlockBounds(lastBlockData);

        OverlapData overlapData = CalculateOverlap(currentBounds, lastBounds);

        if (overlapData.Width <= 0)
        {
            BlockManager.Instance.StopSpawning();
            DropBlock(currentBlock);
            BlockManager.Instance.StopSpawning();
            return false;
        }

        CutData cutData = CalculateCutData(currentBlockData, currentBounds, lastBounds, overlapData);
    
        if (cutData.NewWidth >= minimumVisibleCut)
        {
            PerformBlockCut(currentBlock, cutData);
            
            bool isPerfectCut = IsPerfectCut(cutData.NewWidth, lastBlockData.Scale.x);
            SoundManager.Instance.PlayNoteSound(isPerfectCut);

            return true;
        }
        else
        {
            DropBlock(currentBlock);
            BlockManager.Instance.StopSpawning();
            return false;
        }
    }

    private void PerformBlockCut(BlockController block, CutData cutData)
    {
        Transform mainBlock = block.GetMainBlock();
        Transform cutPiece = block.GetCutPiece();
        Vector3 originalPosition = block.transform.position;
        Vector3 originalScale = mainBlock.localScale;

        mainBlock.localScale = new Vector3(cutData.NewWidth, originalScale.y, originalScale.z);

        float newXPosition;
    
        if (cutData.CutFromRight)
        {
            newXPosition = originalPosition.x - (cutData.CutAmount / 2);
        }
        else
        {
            newXPosition = originalPosition.x + (cutData.CutAmount / 2);
        }

        block.transform.position = new Vector3(newXPosition, originalPosition.y, originalPosition.z);

        cutPiece.gameObject.SetActive(true);

        float cutPieceX = cutData.CutFromRight
            ? originalPosition.x + (originalScale.x / 2) - (cutData.CutAmount / 2) 
            : originalPosition.x - (originalScale.x / 2) + (cutData.CutAmount / 2);

        cutPiece.localScale = new Vector3(cutData.CutAmount, originalScale.y, originalScale.z);
        cutPiece.position = new Vector3(cutPieceX, originalPosition.y, originalPosition.z);

        AddPhysicsToFallingPiece(cutPiece);

        StartCoroutine(CleanupFallingPiece(block, fallingPieceLifetime));
    }

    private void AddPhysicsToFallingPiece(Transform fallingPiece)
    {
        Rigidbody rb = fallingPiece.gameObject.AddComponent<Rigidbody>();
        rb.mass = fallingPieceMass;
        
        rb.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, -3f), Random.Range(-2f, 2f)), ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode.Impulse);
    }

    private IEnumerator CleanupFallingPiece(BlockController block, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Transform cutPiece = block.GetCutPiece();

        Rigidbody rb = cutPiece.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        cutPiece.gameObject.SetActive(false);
        cutPiece.localScale = Vector3.zero;
    }
    
    private bool ValidateBlocks(BlockController currentBlock, BlockController lastBlock)
    {
        if (currentBlock == null || lastBlock == null)
        {
            return false;
        }
        
        if (currentBlock.GetMainBlock() == null || lastBlock.GetMainBlock() == null)
        {
            return false;
        }
        
        if (currentBlock.GetCutPiece() == null)
        {
            return false;
        }
        
        return true;
    }

    private BlockData GetBlockData(BlockController block)
    {
        return new BlockData
        {
            Position = block.transform.position,
            Scale = block.GetMainBlock().localScale
        };
    }

    private BlockBounds CalculateBlockBounds(BlockData blockData)
    {
        float halfWidth = blockData.Scale.x / 2f;
        float left = blockData.Position.x - halfWidth;
        float right = blockData.Position.x + halfWidth;
        
        return new BlockBounds
        {
            Left = left,
            Right = right,
            Width = blockData.Scale.x
        };
    }

    private OverlapData CalculateOverlap(BlockBounds currentBounds, BlockBounds lastBounds)
    {
        float overlapLeft = Mathf.Max(currentBounds.Left, lastBounds.Left);
        float overlapRight = Mathf.Min(currentBounds.Right, lastBounds.Right);
        float overlapWidth = Mathf.Max(0, overlapRight - overlapLeft);
        
        bool hasLeftOverhang = currentBounds.Left < lastBounds.Left;
        bool hasRightOverhang = currentBounds.Right > lastBounds.Right;
        
        float leftOverhang = hasLeftOverhang ? lastBounds.Left - currentBounds.Left : 0;
        float rightOverhang = hasRightOverhang ? currentBounds.Right - lastBounds.Right : 0;
        
        return new OverlapData
        {
            Left = overlapLeft,
            Right = overlapRight,
            Width = overlapWidth,
            LeftOverhang = leftOverhang,
            RightOverhang = rightOverhang,
            HasLeftOverhang = hasLeftOverhang,
            HasRightOverhang = hasRightOverhang
        };
    }
    
    private CutData CalculateCutData(BlockData blockData, BlockBounds currentBounds, BlockBounds lastBounds, OverlapData overlapData)
    {
        bool cutFromRight = overlapData.RightOverhang > overlapData.LeftOverhang;

        float cutAmount;
        float newWidth;
        float newCenterX;

        if (cutFromRight)
        {
            cutAmount = overlapData.RightOverhang;
            newWidth = Mathf.Min(currentBounds.Width - cutAmount, lastBounds.Width);
            newCenterX = lastBounds.Left + (newWidth / 2f);
        }
        else
        {
            cutAmount = overlapData.LeftOverhang;
            newWidth = Mathf.Min(currentBounds.Width - cutAmount, lastBounds.Width); 
            newCenterX = lastBounds.Right - (newWidth / 2f);
        }

        cutAmount = Mathf.Max(cutAmount, minimumVisibleCut);

        return new CutData
        {
            CutFromRight = cutFromRight,
            CutAmount = cutAmount,
            NewWidth = newWidth,
            NewCenterX = newCenterX
        };
    }

   
    #region Helper Structures

    private struct BlockData
    {
        public Vector3 Position;
        public Vector3 Scale;
    }
    
    private struct BlockBounds
    {
        public float Left;
        public float Right;
        public float Width;
    }
    
    private struct OverlapData
    {
        public float Left;
        public float Right;
        public float Width;
        public float LeftOverhang;
        public float RightOverhang;
        public bool HasLeftOverhang;
        public bool HasRightOverhang;
    }
    
    private struct CutData
    {
        public bool CutFromRight;
        public float CutAmount;
        public float NewWidth;
        public float NewCenterX;
    }
    
    #endregion
    
    private void DropBlock(BlockController block)
    {
        Transform mainBlock = block.GetMainBlock();

        Rigidbody rb = mainBlock.gameObject.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), -5f, 0), ForceMode.Impulse);

        StartCoroutine(CleanupFallingPiece(block, 3f));
    }

}