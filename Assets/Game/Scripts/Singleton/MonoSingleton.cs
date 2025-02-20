using UnityEngine;

using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    private static bool applicationIsQuitting = false;
    private static readonly object lockObject = new object();
    
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }
            
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"[Singleton] {typeof(T)}";
                        
                        DontDestroyOnLoad(singletonObject);
                        
                        Debug.Log($"[Singleton] An instance of {typeof(T)} was created.");
                    }
                }
                
                return instance;
            }
        }
    }
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            applicationIsQuitting = true;
        }
    }
    
    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}

