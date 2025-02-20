using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BlockColorManager : MonoSingleton<BlockColorManager>
{
    [SerializeField, Foldout("Color References")] private List<Material> materials;

    public Material GetRandomMaterial()
    {
        return materials[Random.Range(0, materials.Count)];
    }
}
