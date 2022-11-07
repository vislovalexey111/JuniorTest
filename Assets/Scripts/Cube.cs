using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Individual parameters for each cube that are assigned by Cube spawner
    private float _currentDistance;

    private Vector3 _startPosition;
    private float _moveSpeed;
    private float _distance;

    public Vector3 StartPosition
    {
        get { return _startPosition; }
        set { _startPosition = value; }
    }
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = (value <= 0.0f) ? 1.0f : value; }
    }
    public float Distance
    {
        get { return _distance; }
        set { _distance = (value <= 0.0f) ? 1.0f : value; }
    }

    // This action will be called to return object to pool
    public Action<GameObject> OnDistanceEnd;


    private void FixedUpdate()
    {
        _currentDistance = Vector3.Distance(_startPosition, transform.localPosition);
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.forward);
        
        if (_currentDistance >= _distance)
            OnDistanceEnd?.Invoke(gameObject);
    }

    public void SetParameters(Vector3 position, float moveSpeed, float distance)
    {
        StartPosition = position;
        transform.localPosition = position;
        MoveSpeed = moveSpeed;
        Distance = distance;
    }
}
