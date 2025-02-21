using UnityEngine;

public abstract class InitData {}

public interface IPoolable
{
    void Init(InitData data);
    void Reset();
}

public class BlockInitData : InitData
{
    public SpawnSide Side { get; set; }
}