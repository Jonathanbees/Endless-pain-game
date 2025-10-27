using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carAIPrefabs;
    GameObject[] carAIPool = new GameObject[20];
    Transform playerCarTransform;
    float timeLastCarSpawned = 0;
    WaitForSeconds wait = new WaitForSeconds(0.5f);

    // overlapped checked
    [SerializeField]
    LayerMask otherCarsLayerMask;
    Collider[] overlappedCheckCollider = new Collider[1];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
        int prefabIndex = 0;
        for (int i = 0; i < carAIPool.Length; i++)
        {
            carAIPool[i] = Instantiate(carAIPrefabs[prefabIndex]);
            carAIPool[i].SetActive(false);

            prefabIndex++;
            if (prefabIndex >= carAIPrefabs.Length)
                prefabIndex = 0;
        }
        StartCoroutine(UpdateLessOftenCO());

    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            CleanUpCarsBeyondView();
            SpanNewCars();
            yield return wait;
        }
    }
    void SpanNewCars()
    {
        if (Time.time - timeLastCarSpawned < 2)
            return;

        GameObject carToSpawn = null;

        foreach (GameObject aiCar in carAIPool)
        {
            if (aiCar.activeInHierarchy)
            {
                continue;
            }
            carToSpawn = aiCar;
            break;
        }

        if (carToSpawn == null)
            return;

        // Seleccionar un carril aleatorio
        int randomLane = Random.Range(0, RandomizeObject.CarLanes.Length);
        float laneXPosition = RandomizeObject.CarLanes[randomLane];
        
        Vector3 spawnPosition = new Vector3(laneXPosition, 0, playerCarTransform.transform.position.z + 100);

        if (Physics.OverlapBoxNonAlloc(spawnPosition, Vector3.one * 2, overlappedCheckCollider, Quaternion.identity, otherCarsLayerMask) > 0)
        {
            return;
        }

        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);
        timeLastCarSpawned = Time.time;
    }
    void CleanUpCarsBeyondView()
    {
        foreach (GameObject aiCar in carAIPool)
        {
            if (!aiCar.activeInHierarchy)
            {
                continue;
            }
            if (aiCar.transform.position.z - playerCarTransform.position.z > 200)
            {
                aiCar.SetActive(false);
            }
            if (aiCar.transform.position.z - playerCarTransform.position.z < -50)
            {
                aiCar.SetActive(false);
            }
        }
    }
}
