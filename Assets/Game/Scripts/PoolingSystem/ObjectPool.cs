using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private T prefab;
    private Transform parent;
    private Queue<T> pool = new Queue<T>();
    private List<T> activeObjects = new List<T>();
    private int initialSize;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.parent = parent;

        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private T CreateNewObject()
    {
        T newObject = Object.Instantiate(prefab, parent);
        newObject.gameObject.SetActive(false);
        newObject.Reset();
        pool.Enqueue(newObject);
        return newObject;
    }

    public T Get(Vector3 position, Quaternion rotation, params object[] initArgs)
    {
        T objectToReuse;

        if (pool.Count == 0)
        {
            objectToReuse = CreateNewObject();
        }
        else
        {
            objectToReuse = pool.Dequeue();
        }

        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;
        objectToReuse.gameObject.SetActive(true);
     
        objectToReuse.Init(initArgs);

        activeObjects.Add(objectToReuse);

        return objectToReuse;
    }

    public void Return(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        objectToReturn.Reset();
        
        activeObjects.Remove(objectToReturn);
        
        pool.Enqueue(objectToReturn);
    }

    public void ReturnAll()
    {
        foreach (T obj in new List<T>(activeObjects))
        {
            Return(obj);
        }
    }

    public int GetTotalObjectCount()
    {
        return pool.Count + activeObjects.Count;
    }

    public int GetActiveObjectCount()
    {
        return activeObjects.Count;
    }
}