using UnityEngine;
using UnityEngine.UI;

public class ProgressCounter : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Text output;

    [SerializeField]
    string label = "Distancia";

    [SerializeField]
    float unitsToMeters = 1f;

    float startZ;
    int currentMeters;

    public int CurrentMeters => currentMeters;

    /// <summary>
    /// Initializes the starting reference position for progress calculation.
    /// </summary>
    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        startZ = target != null ? target.position.z : 0f;
        UpdateUI(0f);
    }

    /// <summary>
    /// Updates the distance traveled and UI text every frame.
    /// </summary>
    void Update()
    {
        if (output == null)
            return;

        // Try late-binding target if it wasn't ready at Start
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target == null)
        {
            // Still no target; keep showing last value
            return;
        }

        // Prefer CarHandler distance if available (orientation-agnostic)
        var car = target.GetComponent<CarHandler>();
        if (car != null)
        {
            float meters = Mathf.Max(0f, car.DistanceTraveled) * unitsToMeters;
            UpdateUI(meters);
            return;
        }

        // Fallback: world-space Z delta
        float dz = Mathf.Max(0f, target.position.z - startZ);
        float metersFallback = dz * unitsToMeters;
        UpdateUI(metersFallback);
    }

    /// <summary>
    /// Formats and writes the progress value to the assigned UI Text.
    /// </summary>
    /// <param name="meters">Distance in meters to display.</param>
    void UpdateUI(float meters)
    {
        currentMeters = Mathf.FloorToInt(meters);
        output.text = $"{label}: {currentMeters} m";
    }
}

