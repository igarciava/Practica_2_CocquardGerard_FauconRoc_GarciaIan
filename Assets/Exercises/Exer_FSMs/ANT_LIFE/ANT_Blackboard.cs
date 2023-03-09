
using UnityEngine;

public class ANT_Blackboard : MonoBehaviour
{
    [Header("Two point wandering")]
    public GameObject LocationA;
    public GameObject LocationB;
    public float intervalBetweenTimeOuts = 10.0f;
    public float initialSeekWeight = 0.2f;
    public float seekIncrement = 0.2f;
    public float locationReachedRadius = 10.0f;

    [Header("Seed colecting")]
    public GameObject nest;
    public float seedDetectionRadius = 100.0f;
    public float seedReachedRadius = 5.0f;
    public float nestReachedRadius = 20.0f;

    [Header("Peril Fleeing")]
    public float cockDetectionRadius = 50.0f;
    public float cockFarEnoughRadius = 70.0f;
    public GameObject cock;

    void Start()
    {
        cock = GameObject.FindGameObjectWithTag("HEN");
    }

   
}
