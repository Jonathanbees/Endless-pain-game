using UnityEngine;
using UnityEngine.Rendering.Universal;

// Attach this to your Game Over Screen root object.
// It toggles a URP ScriptableRendererFeature (e.g., Fullscreen Pass)
// on when the Game Over UI is shown and off when hidden.
public class GameOverEffectToggle : MonoBehaviour
{
    [Tooltip("Renderer Feature to toggle (assign your Fullscreen Pass feature here)")]
    [SerializeField] ScriptableRendererFeature featureToToggle;

    void OnEnable()
    {
        if (featureToToggle != null)
        {
            featureToToggle.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (featureToToggle != null)
        {
            featureToToggle.SetActive(false);
        }
    }
}

