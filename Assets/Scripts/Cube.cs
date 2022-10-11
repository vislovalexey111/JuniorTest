using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = (value <= 0.0f) ? 1.0f : value; }
    }

    [SerializeField]
    private float distance;
    public float Distance
    {
        get { return distance; }
        set { distance = (value <= 0.0f) ? 1.0f : value; }
    }

    private Vector3 startPosition;
    public Vector3 StartPosition
    {
        get { return startPosition; }
        set { startPosition = value; }
    }

    public static Action<GameObject> OnDistanceEnd;

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(StartPosition, transform.position) >= Distance && OnDistanceEnd != null)
        {
            OnDistanceEnd(gameObject);
        }
    }
}
