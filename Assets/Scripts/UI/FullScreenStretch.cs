using UnityEngine;

// Attach to a UI element with RectTransform (e.g., the Overlay Image)
// Forces anchors to full stretch and zero offsets so it covers the entire Canvas.
public class FullScreenStretch : MonoBehaviour
{
    void Awake()
    {
        ApplyStretch();
    }

    void OnRectTransformDimensionsChange()
    {
        ApplyStretch();
    }

    void ApplyStretch()
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
}

