using UnityEngine;
using UnityEngine.Events;

public class CarCollisionHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody carRigidbody;

    [SerializeField]
    float impactSpeedThreshold = 6f;

    [SerializeField]
    float minApproachAlongNormal = 1f;

    [SerializeField]
    UnityEvent onHeavyImpact;

    /// <summary>
    /// Ensures required components are available.
    /// </summary>
    void Awake()
    {
        if (carRigidbody == null)
            carRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Invokes the configured event when a heavy impact occurs.
    /// </summary>
    /// <param name="collision">Collision data from the physics engine.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (carRigidbody == null || collision.contactCount == 0)
            return;

        float relativeSpeed = collision.relativeVelocity.magnitude;
        if (relativeSpeed < impactSpeedThreshold)
            return;

        Vector3 contactNormal = collision.GetContact(0).normal;
        float approach = Vector3.Dot(carRigidbody.linearVelocity, -contactNormal);
        if (approach < minApproachAlongNormal)
            return;

        onHeavyImpact?.Invoke();
    }
}

