using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    float timer = 0;
    public float minSpawnTime = 20;
    public float maxSpawnTime = 30;
    float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnTime)
        {
            Instantiate(Resources.Load("MOUSE"), RandomLocationGenerator.RandomEnterExitLocation().transform.position, Quaternion.identity);
            timer = 0;
            spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
}
