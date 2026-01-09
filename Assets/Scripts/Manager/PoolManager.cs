using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, object> pools = new Dictionary<string, object>();

    public void CreatePool<T>(T prefab, int initCount, Transform parent = null) where T : MonoBehaviour
    {
        if (prefab == null) return;

        string key = prefab.name;
        if (pools.ContainsKey(key)) return;

        // 새 풀을 만들어 딕셔너리에 등록
        pools.Add(key, new ObjectPool<T>(prefab, initCount, parent));
    }

    public void RemovePool<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null) return;

        string key = prefab.name;
        if (!pools.ContainsKey(key)) return;

        pools.Remove(key);
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab.name, out var box))
        {
            return null;
        }

        var pool = box as ObjectPool<T>;

        if (pool != null)
        {
            return pool.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public void ReturnPool<T>(T instance) where T : MonoBehaviour
    {
        if (instance == null) return;

        if (!pools.TryGetValue(instance.gameObject.name, out var box))
        {
            Destroy(instance.gameObject);
            return;
        }

        var pool = box as ObjectPool<T>;

        if (pool != null)
        {
            pool.Enqueue(instance);
        }
    }
}
