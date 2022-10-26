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
    private float _uiMoveSpeed, _uiMoveDistance, _uiSpawnTime;

    // According to the task, we need to spawn cubes infinitely.
    // For this task better to use object pool (especially on mobile devices).
    private Queue<GameObject> _objectPool;

    // Cashing all "Cube" and "MeshRenderer" components (instead of constantly calling double GetComponent<>() with each "spawn")
    // Enabling/disabling only components we need is about 1.5-2 times faster than original "SetActive" function
    private Dictionary<GameObject, (Cube _cube, MeshRenderer _meshRenderer)> _cubeComponents;
    
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

        // Binding UI events with private methods
        _inputFieldSpeed.onEndEdit.AddListener(delegate { ChangeSpeed(); });
        _inputFieldDistance.onEndEdit.AddListener(delegate { ChangeDistance(); });
        _inputFieldSpawnTime.onEndEdit.AddListener(delegate { ChangeSpawnTime(); });
        _buttonStart.onClick.AddListener(delegate { StartSpawn(); });
        _buttonStop.onClick.AddListener(delegate { StopSpawn(); });

        // Validating and assigning initial UI Input Fields' values
        _uiMoveDistance = ValidUIInputCheck(_inputFieldDistance);
        _uiMoveSpeed = ValidUIInputCheck(_inputFieldSpeed);
        _uiSpawnTime = ValidUIInputCheck(_inputFieldSpawnTime);

        _cubeComponents = new Dictionary<GameObject, (Cube, MeshRenderer)>();
        _objectPool = new Queue<GameObject>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            obj.SetActive(true);
            _cubeComponents.Add(obj,  (obj.GetComponent<Cube>(), obj.GetComponent<MeshRenderer>()));
            SetActiveOvject(obj, false);
            _objectPool.Enqueue(obj);
        }

        _buttonStop.interactable = false;
        Cube.OnDistanceEnd += ReturnToPool;
    }

    // Checks if the UI value is valid and updates UI to show current valid value (if needed).
    private float ValidUIInputCheck(TMP_InputField inputField)
    {
        float tmp = float.Parse(inputField.text);
        tmp = (tmp <= 0.0009f) ? 1.0f : tmp;
        inputField.text = tmp.ToString();

        return tmp;
    }

    private void ReturnToPool(GameObject cube)
    {
        cube.SetActive(false);
        _objectPool.Enqueue(cube);
    }

    // A better replacement to the original SetActive function according to the project requirements.
    private void SetActiveOvject(GameObject gameObject, bool state)
    {
        _cubeComponents[gameObject]._cube.enabled = state;
        _cubeComponents[gameObject]._meshRenderer.enabled = state;
    }

    private void ChangeSpeed() => _uiMoveSpeed = ValidUIInputCheck(_inputFieldSpeed);
    private void ChangeDistance() => _uiMoveDistance = ValidUIInputCheck(_inputFieldDistance);
    private void ChangeSpawnTime() => _uiSpawnTime = ValidUIInputCheck(_inputFieldSpawnTime);

    /*// Set of variables to control tasks
    private bool _isCubeSpawnTaskComplete;
    private Task _cubeSpawnTask;
    
    private void StopSpawn()
    {
        _isCubeSpawnTaskComplete = true;
        _buttonStart.interactable = true;
        _buttonStop.interactable = false;
    }

    private void StartSpawn()
    {
        _buttonStart.interactable = false;
        _buttonStop.interactable = true;
        _isCubeSpawnTaskComplete = false;
        _cubeSpawnTask = new Task(async () =>
        {
            while (!_isCubeSpawnTaskComplete)
            {
                if (_objectPool.Count > 0)
                {
                    GameObject obj = _objectPool.Dequeue();
                    SetActiveOvject(obj, true);
                    obj.transform.localPosition = Vector3.zero;
                    _cubeComponents[obj]._cube.MoveSpeed = _uiMoveSpeed;
                    _cubeComponents[obj]._cube.Distance = _uiMoveDistance;
                }
                await Task.Delay((int)(_uiSpawnTime * 1000.0f));
            }
        });

        // Because of the "SetActive" function, we need to run the task to spawn cubes on the main thread
        _cubeSpawnTask.Start(TaskScheduler.FromCurrentSynchronizationContext());
    }
    
    // If we exit the app/play mode witout tasks been completed, we are going to complete scheduled tasks afterwards.
    // Because of that, we need to change the flag manualy for task completion.
    private void OnApplicationQuit()
    {
        _isCubeSpawnTaskComplete = true;
    }*/

    private Coroutine _cubeCoroutine;

    private void StopSpawn()
    {
        StopCoroutine(_cubeCoroutine);
        _buttonStart.interactable = true;
        _buttonStop.interactable = false;
    }

    private void StartSpawn()
    {
        _buttonStart.interactable = false;
        _buttonStop.interactable = true;
        _cubeCoroutine = StartCoroutine(SpawnCubes());
    }

    private IEnumerator SpawnCubes()
    {
        while (true)
        {
            if (_objectPool.Count > 0)
            {
                GameObject obj = _objectPool.Dequeue();
                SetActiveOvject(obj, true);
                obj.transform.localPosition = Vector3.zero;
                _cubeComponents[obj]._cube.MoveSpeed = _uiMoveSpeed;
                _cubeComponents[obj]._cube.Distance = _uiMoveDistance;
            }
            yield return new WaitForSeconds(_uiSpawnTime);
        }
    }
}