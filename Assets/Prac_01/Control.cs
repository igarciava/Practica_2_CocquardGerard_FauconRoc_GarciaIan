using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    private Camera cam;
    private GameObject wormPrefab;
    private GameObject dummy;
    private GameObject seedPrefab;

    // Start is called before the first frame update
    void Start()
    {
        dummy = new GameObject("dummy");
        cam = Camera.main;
        wormPrefab = Resources.Load<GameObject>("WORM");
        seedPrefab = Resources.Load<GameObject>("SEED");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject worm = GameObject.Instantiate(wormPrefab);
            worm.transform.position = position;
            worm.transform.Rotate(0, 0, Random.value * 360);
            worm.transform.localScale *= 2f;
        }

        if (Input.GetMouseButtonDown(2))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            dummy.transform.position = position;
            GameObject worm = SensingUtils.FindInstanceWithinRadius(dummy, "WORM", 10);
            GameObject seed = SensingUtils.FindInstanceWithinRadius(dummy, "SEED", 10);
            if (worm != null) Destroy(worm);
            if (seed != null) Destroy(seed);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject seed = GameObject.Instantiate(seedPrefab);
            seed.transform.position = position;
            seed.transform.Rotate(0, 0, Random.value * 360);
        }
    }
}
