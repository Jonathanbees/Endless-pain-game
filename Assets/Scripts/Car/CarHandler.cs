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
        }
        if (inputVector.y < 0)
        {
            Brake();
        }
        // Always allow steering alongside acceleration/braking
        Steer();

    }
    void Accelerate()
    {
        carRigidbody.linearDamping = 0;
        carRigidbody.AddForce(transform.forward * acceleration * inputVector.y);
    }
    void Brake(){
        // Apply brake only if moving forward relative to the car's forward
        float forwardSpeed = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);
        if (forwardSpeed <= 0f)
        {
            return;
        }
        // Apply opposing acceleration proportional to brake input
        carRigidbody.AddForce(-transform.forward * brakeForce * Mathf.Abs(inputVector.y), ForceMode.Acceleration);
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
