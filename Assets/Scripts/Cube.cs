using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Individual parameters for each cube that are assigned by Cube spawner
    private float _moveSpeed;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = (value <= 0.0f) ? 1.0f : value; }
    }

    private float _distance;
    public float Distance
    {
        get { return _distance; }
        set { _distance = (value <= 0.0f) ? 1.0f : value; }
    }

    private float _currentPosition;

    // This action will be called to return object to pool
    public static Action<GameObject> OnDistanceEnd;

    private void FixedUpdate()
    {
        _currentPosition = transform.localPosition.magnitude;
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.forward);
        
        if (_currentPosition >= _distance && OnDistanceEnd != null)
            OnDistanceEnd(gameObject);
    }
}
