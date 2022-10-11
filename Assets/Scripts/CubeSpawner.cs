using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeSpawner : MonoBehaviour {
    [SerializeField]
    private TMP_InputField uiSpeed;
    [SerializeField]
    private TMP_InputField uiSpawnTime;
    [SerializeField]
    private TMP_InputField uiDistance;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private float generalMoveSpeed;
    [SerializeField]
    private float generalMoveDistance;
    [SerializeField]
    private float spawnTime;

    [SerializeField]
    private int objectPoolSize;
    private Queue<GameObject> objectPool;
    
    //Cashing all cube data (instead of constantly calling GetComponent)
    private Dictionary<GameObject, Cube> cubeData;
    
    private void Start()
    {
        generalMoveDistance = float.Parse(uiDistance.text);
        generalMoveSpeed = float.Parse(uiSpeed.text);
        spawnTime = float.Parse(uiSpawnTime.text);

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
        StartCoroutine("Spawn");
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
        float tmp = float.Parse(uiSpeed.text);
        generalMoveSpeed = (tmp <= 0.0f) ? 1.0f : tmp;
        uiSpeed.text = generalMoveSpeed.ToString();
    }

    public void ChangeDistance()
    {
        float tmp = float.Parse(uiDistance.text);
        generalMoveDistance = (tmp <= 0.0f) ? 1.0f : tmp;
        uiDistance.text = generalMoveDistance.ToString();
    }

    public void ChangeSpawnTime()
    {
        float tmp = float.Parse(uiSpawnTime.text);
        spawnTime = (tmp <= 0.0f) ? 1.0f : tmp;
        uiSpawnTime.text = spawnTime.ToString();
    }
}
