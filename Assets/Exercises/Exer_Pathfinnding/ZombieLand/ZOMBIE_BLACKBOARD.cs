using UnityEngine;

public class ZOMBIE_BLACKBOARD : MonoBehaviour
{
    public float gutDetectedRadius = 150;
    public float gutReachedRadius = 10;
    public float pointReachedRadius = 3;

    private GameObject[] wanderPoints;
    private GameObject[] collectionPoints;
    
    void Awake()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("WANDERPOINT");
        collectionPoints = GameObject.FindGameObjectsWithTag("COLLECTIONPOINT");
    }

    public GameObject GetRandomWanderPoint ()
    {
        return wanderPoints[Random.Range(0, wanderPoints.Length)];
    }
    public GameObject GetRandomCollectionPoint()
    {
        return collectionPoints[Random.Range(0, collectionPoints.Length)];
    }
}
