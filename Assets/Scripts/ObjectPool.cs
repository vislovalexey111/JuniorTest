using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefabTemplate;
    [SerializeField] private int _poolSize = 10;

    private Queue<GameObject> _pool;
    private Dictionary<GameObject, CubeMover> _cubeComponents;

    public int Count => _pool.Count;

    private void Start()
    {
        if (_prefabTemplate is null)
        {
            Debug.LogError("Prefab template is missing!");
            return;
        }

        _poolSize = (_poolSize < 1) ? 1 : _poolSize;
        _cubeComponents = new ();
        _pool = new (_poolSize);

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_prefabTemplate, transform);
            _cubeComponents.Add(obj, obj.GetComponent<CubeMover>());
            ReturnToPool(obj);
        }
    }

    private void OnDestroy()
    {
        // Getting all cubes, that are not in the pool (are yet subscribed)
        GameObject[] cubes = _cubeComponents.Keys.Where(c => !_pool.Contains(c)).ToArray();
        
        foreach (GameObject c in cubes)
            _cubeComponents[c].OnDistanceEnd -= ReturnToPool;

        _cubeComponents.Clear();
        _pool.Clear();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
        _cubeComponents[obj].OnDistanceEnd -= ReturnToPool;
    }

    public void ReturnFromPool(Vector3 position, float moveSpeed, float moveDistance)
    {
        GameObject obj = _pool.Dequeue();
        obj.SetActive(true);
        _cubeComponents[obj].SetParameters(position, moveSpeed, moveDistance);
        _cubeComponents[obj].OnDistanceEnd += ReturnToPool;
    }
}
