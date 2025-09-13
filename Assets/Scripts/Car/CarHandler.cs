using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody carRigidbody;
    float acceleration = 3;
    float brakeForce = 15;
    float steerForce = 5;
    Vector2 inputVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (inputVector.y > 0)
        {
            Accelerate();
            return;
        }
        if (inputVector.y < 0)
        {
            Brake();
            return;
        }
        Steer();

    }
    void Accelerate()
    {
        carRigidbody.linearDamping = 0;
        carRigidbody.AddForce(transform.forward * acceleration * inputVector.y);
    }
    void Brake(){
        //Dont use brake 
        if (carRigidbody.linearVelocity.z <= 0)
        {
            return;
        }
        carRigidbody.AddForce(carRigidbody.transform.forward * brakeForce * inputVector.y);
    }
    void Steer(){
        if (Mathf.Abs(inputVector.x) > 0)
        {
            carRigidbody.AddForce(transform.right * steerForce * inputVector.x);
        }
        // Removed rotation for endless road game - car should only move laterally
    }
    public void SetInputVector(Vector2 inputVector)
    {
        // Only normalize if there's actual input to avoid residual values
        if (inputVector.magnitude > 0.1f)
        {
            inputVector.Normalize();
        }
        else
        {
            inputVector = Vector2.zero;
        }
        this.inputVector = inputVector;
    }
}
