using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class ObjectPlacer : MonoBehaviour
{
    private ObjectPool _objectPool;

    private float _uiMoveSpeed;
    private float _uiMoveDistance;
    private float _uiSpawnTime;

    public float UiMoveSpeed { set => _uiMoveSpeed = (value < 0.001f) ? 1.0f : value; }
    public float UiMoveDistance { set => _uiMoveDistance = (value < 0.001f) ? 1.0f : value; }
    public float UiSpawnTime { set => _uiSpawnTime = (value < 0.001f) ? 1.0f : value; }

    private void Start() => _objectPool = GetComponent<ObjectPool>();

    public IEnumerator PlaceCube()
    {
        while (true)
        {
            if (_objectPool.Count > 0)
                _objectPool.ReturnFromPool(Vector3.zero, _uiMoveSpeed, _uiMoveDistance);

            yield return new WaitForSeconds(_uiSpawnTime);
        }
    }
}
