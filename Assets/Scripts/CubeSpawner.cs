using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeSpawner : MonoBehaviour
{
    /*
     * While we editing text, we should not assign values from UI to "spawned" cube directly.
     * Better to store those values in spawner general parameters after we finished
     * editing text to make sure that each value is correct.
    */
    [SerializeField]
    private TMP_InputField _uiSpeed, _uiSpawnTime, _uiDistance;

    // General spawner parameters to validate and store UI Input Fields' values.
    private float _generalMoveSpeed, _generalMoveDistance, _spawnTime;

    /*
     * Because of the task, we need to spawn cubes infinitely.
     * For this task better to use object pool (especially on mobile devices).
    */
    private Queue<GameObject> _objectPool;

    // Cashing all "Cube" components (instead of constantly calling GetComponent<Cube>() with each "spawn").
    private Dictionary<GameObject, Cube> _cubeComponents;

    private Coroutine _cubeCoroutine;
    private bool _failedToSetElements;

    private void Start()
    {
        // Checking if all elements are set in the inspector.
        _failedToSetElements = !_uiSpeed || !_uiSpawnTime || !_uiDistance || transform.childCount == 0;

        if (_failedToSetElements)
        {
            Debug.LogError("Not all elemets are set in the inspector!");
            return;
        }

        _generalMoveDistance = ValidUIInputCheck(_uiDistance);
        _generalMoveSpeed = ValidUIInputCheck(_uiSpeed);
        _spawnTime = ValidUIInputCheck(_uiSpawnTime);

        _cubeComponents = new Dictionary<GameObject, Cube>();
        _objectPool = new Queue<GameObject>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            _cubeComponents.Add(obj, obj.GetComponent<Cube>());
            _objectPool.Enqueue(obj);
        }

        Cube.OnDistanceEnd += ReturnToPool;
    }
    
    private IEnumerator Spawn()
    {
        if (_failedToSetElements)
        {
            Debug.LogError("Not all elemets are set in the inspector!");
            yield break;
        }

        while (true)
        {
            yield return new WaitForSeconds(_spawnTime);

            if (_objectPool.Count > 0)
            {
                GameObject obj = _objectPool.Dequeue();
                obj.SetActive(true);
                obj.transform.localPosition = Vector3.zero;
                _cubeComponents[obj].MoveSpeed = _generalMoveSpeed;
                _cubeComponents[obj].Distance = _generalMoveDistance;
            }
        }
    }

    // Checks if the UI value is valid and updates UI to show current valid value (if needed).
    private float ValidUIInputCheck(TMP_InputField inputField)
    {
        float tmp = float.Parse(inputField.text);

        if (tmp <= 0.0f)
        {
            tmp = 1.0f;
            inputField.text = tmp.ToString();
        }

        return tmp;
    }

    private void ReturnToPool(GameObject cube)
    {
        cube.SetActive(false);
        _objectPool.Enqueue(cube);
    }

    public void StartSpawning()
    {
        _cubeCoroutine = StartCoroutine(Spawn());
    }

    public void StopSpawning()
    {
        if (_cubeCoroutine != null)
            StopCoroutine(_cubeCoroutine);
    }

    public void ChangeSpeed()
    {
        _generalMoveSpeed = ValidUIInputCheck(_uiSpeed);
    }

    public void ChangeDistance()
    {
        _generalMoveDistance = ValidUIInputCheck(_uiDistance);
    }

    public void ChangeSpawnTime()
    {
        _spawnTime = ValidUIInputCheck(_uiSpawnTime);
    }
}