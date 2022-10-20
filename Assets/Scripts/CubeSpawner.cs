using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeSpawner : MonoBehaviour {
    /*
     * We should not assign values from UI to spawn prefab directly while we editing text.
     * So we store those values in spawner general parameters when we finished editing text.
    */
    [SerializeField]
    private TMP_InputField uiSpeed, uiSpawnTime, uiDistance;

    [SerializeField]
    private GameObject prefab;

    
    // General spawner parameters to check if information in UI is valid and to assign valid parameters;
    [SerializeField]
    private float generalMoveSpeed, generalMoveDistance, spawnTime;

    /*
     * Because of the task, we need to spawn infinite amount of cubes.
     * For this task better to use object pool (especially on mobile devices)
    */
    [SerializeField]
    private int objectPoolSize = 100;
    private Queue<GameObject> objectPool;
    
    // Cashing all Cube data (instead of constantly calling GetComponent with each "spawn")
    private Dictionary<GameObject, Cube> cubeData;
    
    private void Start()
    {
        // Checking if start parameters are valid
        objectPoolSize = (objectPoolSize < 1) ? 1 : objectPoolSize;
        generalMoveDistance = ValidUIInputCheck(uiDistance);
        generalMoveSpeed = ValidUIInputCheck(uiSpeed);
        spawnTime = ValidUIInputCheck(uiSpawnTime);

        cubeData = new Dictionary<GameObject, Cube>();
        objectPool = new Queue<GameObject>();

        for (int i = 0; i < objectPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            cubeData.Add(obj, obj.GetComponent<Cube>());
            objectPool.Enqueue(obj);
        }

        Cube.OnDistanceEnd += ReturnToPool;
        StartCoroutine(nameof(Spawn));
    }

    private IEnumerator Spawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnTime);

            if(objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                obj.transform.position = transform.position;
                cubeData[obj].StartPosition = transform.position;
                cubeData[obj].MoveSpeed = generalMoveSpeed;
                cubeData[obj].Distance = generalMoveDistance;
            }
        }
    }

    private void ReturnToPool(GameObject _cube)
    {
        _cube.SetActive(false);
        objectPool.Enqueue(_cube);
    }

    public void ChangeSpeed()
    {
        generalMoveSpeed = ValidUIInputCheck(uiSpeed);
    }

    public void ChangeDistance()
    {
        generalMoveDistance = ValidUIInputCheck(uiDistance);
    }

    public void ChangeSpawnTime()
    {
        spawnTime = ValidUIInputCheck(uiSpawnTime);
    }

    // Checks if the UI value is valid and updates UI to show current valid value (if needed)
    private float ValidUIInputCheck(TMP_InputField inputField)
    {
        float tmp = float.Parse(inputField.text);

        if(tmp <= 0.0f)
        {
            tmp = 1.0f;
            inputField.text = tmp.ToString();
        }
        
        return tmp;
    }
}
