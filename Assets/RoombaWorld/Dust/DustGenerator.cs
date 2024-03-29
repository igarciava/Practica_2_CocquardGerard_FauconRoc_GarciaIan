using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustGenerator : MonoBehaviour
{
    float timer = 0;
    public float spawnTime = 5;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > spawnTime)
        {
            GameObject dust = (GameObject)Instantiate(Resources.Load("DUST"), RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
            dust.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
            timer = 0;
        }
    }
}
