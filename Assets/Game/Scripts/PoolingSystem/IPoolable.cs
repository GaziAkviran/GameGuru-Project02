using UnityEngine;

public interface IPoolable
{
    void Init(params object[] args);
    void Reset();
}
