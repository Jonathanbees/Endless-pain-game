using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;

public class AIHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    [SerializeField]
    LayerMask otherCarsLayer;

    [SerializeField]
    MeshCollider meshCollider;

    //Collision detection to prevent AI on player car
    RaycastHit[] raycastHits = new RaycastHit[1];
    bool isCarAhead = false;

    //lanes
    int drivingLane = 0;

    //timing 
    WaitForSeconds wait = new WaitForSeconds(0.2f);
    private void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        StartCoroutine(UpdateLessOftenCO());
    }

    // Update is called once per frame
    void Update()
    {
        float accelerationInput = 1f; // Always accelerate
        float steeringInput = Mathf.Sin(Time.time); // Simple oscillating steering

        if (isCarAhead)
        {
            accelerationInput = -1f; // Stop accelerating if car ahead
        }
        float desiredPositionX = RandomizeObject.CarLanes[drivingLane];
        float difference = desiredPositionX - transform.localPosition.x;

        if (Mathf.Abs(difference) > 0.05f)
        {
            steeringInput = 1f * difference; // Steer towards the lane
        }
        else
        {
            steeringInput = 0f;
        }

        Vector2 inputVector = new Vector2(steeringInput, accelerationInput);
        carHandler.SetInputVector(inputVector);
        carHandler.SetMaxAutoSpeed(Random.Range(5, 10));

    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            isCarAhead = CheckIfOtherCarsAhead();
            yield return wait;
        }
    }

    bool CheckIfOtherCarsAhead()
    {
        meshCollider.enabled = false;
        int numberOfHits = Physics.BoxCastNonAlloc(transform.position + transform.up * 0.5f, Vector3.one * 0.25f, transform.forward, raycastHits, Quaternion.identity, 4, otherCarsLayer);
        meshCollider.enabled = true;

        if (numberOfHits > 0)
        {
            return true;
        }
        return false;
    }


    private void OnEnable()
    {
        //random speed
        carHandler.SetMaxAutoSpeed(Random.Range(5, 10));
        
        //random lane
        drivingLane = Random.Range(0, RandomizeObject.CarLanes.Length);
    }
}
