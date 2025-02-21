using System.Collections;
using UnityEngine;

public class PlayerFallController : MonoBehaviour
{
    [SerializeField] private float fallDetectionDistance = 1.5f;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private PlayerAnimationController animationController;
    
    private bool isFalling = false;
    private GameStateManager gameStateManager;

    private void Start()
    {
        gameStateManager = GameStateManager.Instance;
    }

    private void FixedUpdate()
    {
        if (gameStateManager.CurrentState != GameState.Gameplay) return;
        CheckForFall();
    }

    private void CheckForFall()
    {
        if (isFalling) return;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        RaycastHit hit;

        if (!Physics.Raycast(rayOrigin, Vector3.down, out hit, fallDetectionDistance, blockLayer))
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        isFalling = true;
        animationController.PlayFallAnimation(); 
        gameStateManager.LoseGame();
    }
    
}
