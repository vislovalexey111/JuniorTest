using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CubeSpawner : MonoBehaviour
{
    /*
     * While we editing text, we should not assign values from UI to "spawned" cube directly.
     * Better to store those values in spawner general parameters after we finished
     * editing text to make sure that each value is correct.
    */
    [SerializeField]
    private Button _buttonStart, _buttonStop;

    [SerializeField]
    private TMP_InputField _inputFieldSpeed, _inputFieldSpawnTime, _inputFieldDistance;

    // General spawner parameters to validate and store UI Input Fields' values.
    private float _generalMoveSpeed, _generalMoveDistance, _spawnTime;

    /*
     * According to the task, we need to spawn cubes infinitely.
     * For this task better to use object pool (especially on mobile devices).
    */
    private Queue<GameObject> _objectPool;

    // Cashing all "Cube" components (instead of constantly calling GetComponent<Cube>() with each "spawn").
    private Dictionary<GameObject, Cube> _cubeComponents;

    private Coroutine _cubeSpawnCoroutine;

    private void Start()
    {
        // Validating script references.
        if(transform.childCount == 0)
        {
            Debug.LogError("This object doesn't have child \'Cubes\' to pool!");
            return;
        }
        if (!_inputFieldSpeed
            || !_inputFieldSpawnTime
            || !_inputFieldDistance
            || !_buttonStart
            || !_buttonStop
            )
        {
            Debug.LogError("Not all elemets are set for \'Spawner\' in the inspector!");
            return;
        }

        // Binding UI events with methods
        _inputFieldSpeed.onEndEdit.AddListener(delegate { ChangeSpeed(); });
        _inputFieldDistance.onEndEdit.AddListener(delegate { ChangeDistance(); });
        _inputFieldSpawnTime.onEndEdit.AddListener(delegate { ChangeSpawnTime(); });
        _buttonStart.onClick.AddListener(delegate { StartSpawn(); });
        _buttonStop.onClick.AddListener(delegate { StopSpawn(); });

        // Validating and assigning initial UI Input Fields' values
        _generalMoveDistance = ValidUIInputCheck(_inputFieldDistance);
        _generalMoveSpeed = ValidUIInputCheck(_inputFieldSpeed);
        _spawnTime = ValidUIInputCheck(_inputFieldSpawnTime);

        _cubeComponents = new Dictionary<GameObject, Cube>();
        _objectPool = new Queue<GameObject>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            _cubeComponents.Add(obj, obj.GetComponent<Cube>());
            _objectPool.Enqueue(obj);
        }

        _buttonStop.interactable = false;
        Cube.OnDistanceEnd += ReturnToPool;
    }
    // Checks if the UI value is valid and updates UI to show current valid value (if needed).
    private float ValidUIInputCheck(TMP_InputField inputField)
    {
        float tmp = float.Parse(inputField.text);
        tmp = (tmp <= 0.0f) ? 1.0f : tmp;
        inputField.text = tmp.ToString();

        return tmp;
    }

    private void ReturnToPool(GameObject cube)
    {
        cube.SetActive(false);
        _objectPool.Enqueue(cube);
    }

    private void StopSpawn()
    {
        _buttonStart.interactable = true;
        _buttonStop.interactable = false;

        StopCoroutine(_cubeSpawnCoroutine);
    }

    private void StartSpawn()
    {
        _buttonStart.interactable = false;
        _buttonStop.interactable = true;

        _cubeSpawnCoroutine = StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            if (_objectPool.Count > 0)
            {
                GameObject obj = _objectPool.Dequeue();
                obj.SetActive(true);
                obj.transform.localPosition = Vector3.zero;
                _cubeComponents[obj].MoveSpeed = _generalMoveSpeed;
                _cubeComponents[obj].Distance = _generalMoveDistance;
            }

            yield return new WaitForSeconds(_spawnTime);
        }
    }
    
    /*
    // The coroutine replaced with the async/await
    private Task _cubeSpawnTask;
    private bool _isCubeSpawnTaskComplete = false;

    private void StopSpawn()
    {
        _isCubeSpawnTaskComplete = true;
        Task.WaitAll(_cubeSpawnTask);

        _buttonStart.interactable = true;
        _buttonStop.interactable = false;
    }

    private void StartSpawn()
    {
        _buttonStart.interactable = false;
        _buttonStop.interactable = true;

        _isCubeSpawnTaskComplete = false;
        _cubeSpawnTask = CubeSpawnTask();

        // We need to start Task on the main thread because of the SetActive function
        _cubeSpawnTask.Start(TaskScheduler.FromCurrentSynchronizationContext());
    }

    private Task CubeSpawnTask()
    {
        return new Task(async () =>
        {
            while (!_isCubeSpawnTaskComplete)
            {
                if (_objectPool.Count > 0)
                {
                    GameObject obj = _objectPool.Dequeue();
                    obj.SetActive(true);
                    obj.transform.localPosition = Vector3.zero;
                    _cubeComponents[obj].MoveSpeed = _generalMoveSpeed;
                    _cubeComponents[obj].Distance = _generalMoveDistance;
                }

                await Task.Delay((int)_spawnTime * 1000);
            }
        });
    }
    */
    private void ChangeSpeed() => _generalMoveSpeed = ValidUIInputCheck(_inputFieldSpeed);
    private void ChangeDistance() => _generalMoveDistance = ValidUIInputCheck(_inputFieldDistance);
    private void ChangeSpawnTime() => _spawnTime = ValidUIInputCheck(_inputFieldSpawnTime);
}