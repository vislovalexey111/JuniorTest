using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    private int _poolSize;
    private readonly Queue<GameObject> _objectPool;

    public int PoolSize
    {
        get { return _poolSize; }
    }
    public int PoolCount
    {
        get { return _objectPool.Count; }
    }

    public ObjectPooler(int poolSize = 10)
    {
        _poolSize = (poolSize < 1) ? 1 : poolSize;
        _objectPool = new Queue<GameObject>(_poolSize);
    }

    ~ObjectPooler()
    {
        _objectPool.Clear();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        _objectPool.Enqueue(obj);
    }

    public GameObject ReturnFromPool()
    {
        GameObject obj = _objectPool.Dequeue();
        obj.SetActive(true);
        return obj;
    }
}
