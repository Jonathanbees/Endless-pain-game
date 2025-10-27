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
}
