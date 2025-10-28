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
    public UnityEvent onHeavyImpact;

    [Header("Debug")]
    [SerializeField]
    bool debugLogs = false;

    [Header("Approach Filter")]
    [Tooltip("If enabled, requires a minimum closing speed along the contact normal to qualify as heavy impact.")]
    [SerializeField]
    bool requireApproachAlongNormal = true;

    /// <summary>
    /// Ensures required components are available.
    /// </summary>
    void Awake()
    {
        if (carRigidbody == null)
            carRigidbody = GetComponent<Rigidbody>();

        // Ensure the event is initialized when this component is added at runtime
        if (onHeavyImpact == null)
        {
            onHeavyImpact = new UnityEvent();
        }
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
        if (debugLogs)
        {
            Debug.Log($"[CarCollisionHandler] Collision with {collision.gameObject.name}. relativeSpeed={relativeSpeed:F2}, threshold={impactSpeedThreshold:F2}");
        }
        if (relativeSpeed < impactSpeedThreshold)
        {
            if (debugLogs) Debug.Log("[CarCollisionHandler] Below impact speed threshold. Ignored.");
            return;
        }

        Vector3 contactNormal = collision.GetContact(0).normal;
        // Use relative velocity along the contact normal to measure closing speed into the obstacle.
        // The sign can vary depending on collider order, so compare using absolute magnitude.
        float approachSigned = Vector3.Dot(collision.relativeVelocity, -contactNormal);
        float approach = Mathf.Abs(approachSigned);
        if (debugLogs)
        {
            float approachNorm = relativeSpeed > 0.0001f ? Mathf.Clamp01(approach / relativeSpeed) : 0f;
            Debug.Log($"[CarCollisionHandler] approachAlongNormal signed={approachSigned:F2}, abs={approach:F2} (norm={approachNorm:P0}), minRequired={minApproachAlongNormal:F2}, require={requireApproachAlongNormal}");
        }
        if (requireApproachAlongNormal && approach < minApproachAlongNormal)
        {
            if (debugLogs) Debug.Log("[CarCollisionHandler] Impact not sufficiently head-on. Ignored due to approach filter.");
            return;
        }

        if (debugLogs) Debug.Log("[CarCollisionHandler] Heavy impact detected. Invoking onHeavyImpact.");
        onHeavyImpact?.Invoke();
    }
}

