using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabTemplate;

    [SerializeField]
    private int _spawnCount = 10;

    // General parameters to validate and store UI Input Fields' values.
    private float _uiMoveSpeed, _uiMoveDistance, _uiSpawnTime;

    // Properties we can access in UIWrapper
    public float UiMoveSpeed
    {
        set { _uiMoveSpeed = (value < 0.001f) ? 1.0f : value; }
    }
    public float UiMoveDistance
    {
        set { _uiMoveDistance = (value < 0.001f) ? 1.0f : value; }
    }
    public float UiSpawnTime
    {
        set { _uiSpawnTime = (value < 0.001f) ? 1.0f : value; }
    }

    // According to the task, we need to spawn cubes infinitely.
    // For this task better to use object pool (especially on mobile devices).
    private ObjectPooler _objectPooler;
    private Dictionary<GameObject, Cube> _cubeComponents;

    private void Start()
    {
        // Validating reference.
        if (!_prefabTemplate)
        {
            Debug.LogError("Prefab template is missing!");
            return;
        }

        _objectPooler = new ObjectPooler(_spawnCount);
        _cubeComponents = new Dictionary<GameObject, Cube>();

        for (int i = 0; i < _objectPooler.PoolSize; i++)
        {
            GameObject obj = Instantiate(_prefabTemplate);
            obj.transform.parent = transform;
            _cubeComponents.Add(obj, obj.GetComponent<Cube>());
            _cubeComponents[obj].OnDistanceEnd += _objectPooler.ReturnToPool;
            _objectPooler.ReturnToPool(obj);
        }
    }

    private void OnDestroy()
    {
        foreach(Cube cubeComponent in _cubeComponents.Values)
            cubeComponent.OnDistanceEnd -= _objectPooler.ReturnToPool;
    }

    public IEnumerator SpawnCube()
    {
        while (true)
        {
            if (_objectPooler.PoolCount > 0)
            {
                GameObject obj = _objectPooler.ReturnFromPool();
                _cubeComponents[obj].SetParameters(Vector3.zero, _uiMoveSpeed, _uiMoveDistance);
            }

            yield return new WaitForSeconds(_uiSpawnTime);
        }
    }
}