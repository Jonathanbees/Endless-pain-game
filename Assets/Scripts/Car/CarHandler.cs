using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody carRigidbody;
    float acceleration = 3;
    float brakeForce = 15;
    float steerForce = 5;
    
    // Auto acceleration settings
    [SerializeField]
    float autoAcceleration = 2f;
    [SerializeField]
    float maxAutoSpeed = 15f;

    Vector2 inputVector;

    //audio

    [Header("SFX")]
    [SerializeField]
    AudioSource carEngineAS;

    [SerializeField]
    AudioSource carSkidAS;

    [SerializeField]
    AnimationCurve carPitchAnimationCurve; // Pitch changes based on speed, for example, de engine increase its sound when the car speed up

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //Stats
    float carStartPositionZ;
    float distanceTraveled = 0;

    public float DistanceTraveled => distanceTraveled;


    //exploded state
    bool isExploded = false;
    bool isPlayer; // Remover el = true

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        
        // Determinar si es jugador basado en el tag
        isPlayer = CompareTag("Player");

        if (isPlayer && carEngineAS != null)
        {
            carEngineAS.Play();
        }
        carStartPositionZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploded)
        {
            FadeOutCarAudio();
            return; 
        }

        UpdateCarAudio();

        //updated distance traveled
        distanceTraveled = transform.position.z - carStartPositionZ;
    }
    void FixedUpdate()
    {
        if (isExploded)
        {
            // Prevent further movement forces after explosion/game over
            carRigidbody.linearDamping = 5f;
            return;
        }
        // Auto acceleration (always active unless braking)
        if (inputVector.y >= 0) // Only auto-accelerate when not pressing brake (down arrow)
        {
            AutoAccelerate();
        }
        
        // Manual acceleration boost
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
    
    void AutoAccelerate()
    {
        // Get current forward speed
        float currentSpeed = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);
        
        // Only auto-accelerate if below max speed
        if (currentSpeed < maxAutoSpeed)
        {
            carRigidbody.linearDamping = 0;
            carRigidbody.AddForce(transform.forward * autoAcceleration);
        }
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
    void Steer()
    {
        if (Mathf.Abs(inputVector.x) > 0)
        {
            carRigidbody.AddForce(transform.right * steerForce * inputVector.x);
        }
        // Removed rotation for endless road game - car should only move laterally
    }

    void UpdateCarAudio()
    {
        if (!isPlayer)
        {
            return;
        }
        
        // Solo actualizar el pitch del motor si carEngineAS existe
        if (carEngineAS != null)
        {
            float carMaxSpeedPercentage = carRigidbody.linearVelocity.magnitude / maxAutoSpeed;
            carEngineAS.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeedPercentage);
        }

        // Skid sound logic - solo si carSkidAS existe
        if (carSkidAS != null)
        {
            if (inputVector.y < 0 && carRigidbody.linearVelocity.magnitude > 1f)
            {
                if (!carSkidAS.isPlaying)
                {
                    carSkidAS.Play();
                }
                carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 1f, Time.deltaTime * 10);
            }
            else
            {
                carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0f, Time.deltaTime * 30);
            }
        }
    }
    
    void FadeOutCarAudio()
    {
        if (!isPlayer) return;
        
        if (carEngineAS != null)
        {
            carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0f, Time.deltaTime * 10);
        }
        
        if (carSkidAS != null)
        {
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0f, Time.deltaTime * 10);
        }
    }
    public void SetInputVector(Vector2 inputVector)
    {
        // Solo aceptar input si es el jugador
        if (!isPlayer)
        {
            return; // Los carros IA ignoran el input manual
        }
        
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
    public void SetMaxAutoSpeed(float newMaxSpeed)
    {
        maxAutoSpeed = newMaxSpeed;
    }

    // Called on Game Over / explosion to stop control and fade audio
    public void TriggerExploded()
    {
        isExploded = true;
        // Clear inputs to avoid residual steering/forces
        inputVector = Vector2.zero;
        Debug.Log("[CarHandler] TriggerExploded: Disabling forces and fading audio.");
    }
}
