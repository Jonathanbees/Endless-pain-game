using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text distanceText;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    [Header("Debug")]
    [SerializeField] bool debugLogs = false;

    void OnEnable()
    {
        // Ensure full-screen stretch when the overlay is shown
        TryStretchToFullScreen();

        // Placeholder distance until integrated with scoring
        if (distanceText != null) distanceText.text = "Distancia: ";

        // Focus restart for keyboard/controller
        if (restartButton != null)
        {
            var es = EventSystem.current;
            if (es != null) es.SetSelectedGameObject(restartButton.gameObject);
        }
    }

    public void OnClickRestart()
    {
        if (debugLogs) Debug.Log("[GameOverUI] Restart clicked.");
        Time.timeScale = 1f;
        // Reload current scene to avoid relying on build index order
        var scene = SceneManager.GetActiveScene();
        if (scene.IsValid()) SceneManager.LoadScene(scene.buildIndex);
    }

    public void OnClickQuit()
    {
        if (debugLogs) Debug.Log("[GameOverUI] Quit clicked.");
        Application.Quit();
    }

    // Optional API for future integration
    public void SetDistance(int meters)
    {
        if (distanceText != null)
        {
            distanceText.text = $"Distancia: {meters} m";
        }
    }

    // Forces the root panel (under the Canvas) to cover the entire screen
    void TryStretchToFullScreen()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        // Ascend to the highest RectTransform that is a direct child of the Canvas
        RectTransform target = transform as RectTransform;
        Transform canvasTr = canvas.transform;
        Transform tr = transform;
        RectTransform candidate = target;
        while (tr != null && tr.parent != null && tr.parent != canvasTr)
        {
            tr = tr.parent;
            candidate = tr as RectTransform;
        }

        if (tr != null && tr.parent == canvasTr)
        {
            target = candidate != null ? candidate : target;
        }

        if (target == null) return;

        target.anchorMin = Vector2.zero;
        target.anchorMax = Vector2.one;
        target.offsetMin = Vector2.zero;
        target.offsetMax = Vector2.zero;
        target.pivot = new Vector2(0.5f, 0.5f);
        target.localScale = Vector3.one;
    }
}
