
using UnityEngine;


public class SpaceOnOff : MonoBehaviour
{
    private float timeScale;


    void Awake()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Update()
    {
        if (Input.GetKeyDown("space"))
            if (Time.timeScale == 0f) Time.timeScale = timeScale;
            else Time.timeScale = 0f;
    }
}
