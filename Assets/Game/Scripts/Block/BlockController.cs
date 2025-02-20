using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BlockController : MonoBehaviour, IPoolable
{
    [SerializeField, Foldout("References")] private MeshRenderer meshRenderer;


    public void Init(params object[] args)
    {
        Reset();
        AssignRandomMaterial();
    }

    public void Reset()
    {
        
    }

    private void AssignRandomMaterial()
    {
        meshRenderer.material = BlockColorManager.Instance.GetRandomMaterial();
    }
}
