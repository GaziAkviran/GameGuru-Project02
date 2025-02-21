using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private Dictionary<string, object> pools = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public ObjectPool<T> GetOrCreatePool<T>(T prefab, int initialSize, Transform parent = null) where T : MonoBehaviour, IPoolable
    {
        
        string key = typeof(T).Name + "_" + prefab.name;
        
        if (pools.ContainsKey(key))
        {
            return (ObjectPool<T>)pools[key];
        }

        ObjectPool<T> newPool = new ObjectPool<T>(prefab, initialSize, parent);
        pools.Add(key, newPool);

        return newPool;
    }

    public T GetFromPool<T>(T prefab, Vector3 position, Quaternion rotation, InitData initData) where T : MonoBehaviour, IPoolable
    {
        ObjectPool<T> pool = GetOrCreatePool(prefab, 10, null);
        return pool.Get(position, rotation, initData);
    }

    public void ReturnToPool<T>(T objectToReturn) where T : MonoBehaviour, IPoolable
    {
        string key = typeof(T).Name + "_" + objectToReturn.name.Replace("(Clone)", "").Trim();

        if (pools.ContainsKey(key))
        {
            ((ObjectPool<T>)pools[key]).Return(objectToReturn);
        }
        else
        {
            Debug.LogWarning($"Pool for {key} not found!");
        }
    }
}
