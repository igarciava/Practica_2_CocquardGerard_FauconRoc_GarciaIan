using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustGenerator : MonoBehaviour
{
    public GameObject Dust;
    private RandomLocationGenerator Generator;

    // Start is called before the first frame update
    void Start()
    {
        Generator = new RandomLocationGenerator();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
