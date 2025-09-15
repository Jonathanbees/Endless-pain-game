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
        if (target == null || output == null)
            return;

        float dz = Mathf.Max(0f, target.position.z - startZ);
        float meters = dz * unitsToMeters;
        UpdateUI(meters);
    }

    /// <summary>
    /// Formats and writes the progress value to the assigned UI Text.
    /// </summary>
    /// <param name="meters">Distance in meters to display.</param>
    void UpdateUI(float meters)
    {
        int whole = Mathf.FloorToInt(meters);
        output.text = $"{label}: {whole} m";
    }
}

