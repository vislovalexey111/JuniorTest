using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ObjectSpawner))]
public class UIWrapper : MonoBehaviour
{
    /* While we editing text, we should not assign values from UI to "spawned" cube directly.
     * Better to store those values in spawner general parameters after we finished
     * editing text to make sure that each value is correct. */
    [SerializeField]
    private TMP_InputField _inputFieldSpeed, _inputFieldSpawnTime, _inputFieldDistance;

    [SerializeField]
    private Button _buttonStart, _buttonStop;

    private ObjectSpawner _objectSpawner;
    private Coroutine _cubeSpawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (!_inputFieldSpeed
            || !_inputFieldSpawnTime
            || !_inputFieldDistance
            || !_buttonStart
            || !_buttonStop)
        {
            Debug.LogError("Not all elemets are set for \'UI Wrapper\' in the inspector!");
            return;
        }

        // Binding UI events with private methods
        _inputFieldSpeed.onEndEdit.AddListener(delegate { SetSpeed(); });
        _inputFieldDistance.onEndEdit.AddListener(delegate { SetDistance(); });
        _inputFieldSpawnTime.onEndEdit.AddListener(delegate { SetSpawnTime(); });
        _buttonStart.onClick.AddListener(delegate { StartSpawn(); });
        _buttonStop.onClick.AddListener(StopSpawn);

        _objectSpawner = GetComponent<ObjectSpawner>();

        // Validating and assigning initial UI Input Fields' values
        SetSpeed();
        SetDistance();
        SetSpawnTime();

        _buttonStop.interactable = false;
    }

    private void OnDestroy()
    {
        _inputFieldSpeed.onEndEdit.RemoveAllListeners();
        _inputFieldDistance.onEndEdit.RemoveAllListeners();
        _inputFieldSpawnTime.onEndEdit.RemoveAllListeners();
        _buttonStart.onClick.RemoveAllListeners();
        _buttonStop.onClick.RemoveAllListeners();
    }

    // Checks if the UI value is valid and updates UI to show current valid value (if needed).
    private float ValidUIInputCheck(TMP_InputField inputField)
    {
        float tmp = float.Parse(inputField.text);
        tmp = (tmp < 0.001f) ? 1.0f : tmp;
        inputField.text = tmp.ToString();

        return tmp;
    }

    private void StopSpawn()
    {
        StopCoroutine(_cubeSpawnCoroutine);
        _buttonStart.interactable = true;
        _buttonStop.interactable = false;
    }

    private void StartSpawn()
    {
        _buttonStart.interactable = false;
        _buttonStop.interactable = true;
        _cubeSpawnCoroutine = StartCoroutine(_objectSpawner.SpawnCube());
    }

    private void SetSpeed() => _objectSpawner.UiMoveSpeed = ValidUIInputCheck(_inputFieldSpeed);
    private void SetDistance() => _objectSpawner.UiMoveDistance = ValidUIInputCheck(_inputFieldDistance);
    private void SetSpawnTime() => _objectSpawner.UiSpawnTime = ValidUIInputCheck(_inputFieldSpawnTime);
}