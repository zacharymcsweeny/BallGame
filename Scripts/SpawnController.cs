using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public float spawnTimer = 3f;
    public float currentSpawnTimer = 0f;
    public int totalSpawnLimit = -1;
    public int concurrentSpawnLimit = 1;

    public float spawningOffsetX = 0f;
    public float spawningOffsetY = 0f;
    public float spawningOffsetZ = 0f;

    [SerializeField]
    GameObject spawnedEntity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentSpawnTimer -= Time.deltaTime;
        if (currentSpawnTimer < 0)
        {
            Spawn();
            currentSpawnTimer = spawnTimer;
        }
    }

    void Spawn()
    {
        GameObject spawn = (GameObject)Instantiate(spawnedEntity, transform.position + new Vector3(spawningOffsetX, spawningOffsetY, spawningOffsetZ), Quaternion.identity);
    }
}
