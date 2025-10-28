using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiHandler : MonoBehaviour
{
    void Start()
    {
        
    }

    public void OnStartGameClicked()
    {
        // La variable estática selectedCarIndex ya está guardada
        Debug.Log($"Iniciando juego con carro índice: {PlayerCarSpawner.selectedCarIndex}");
        SceneManager.LoadScene("Stage");
    }
}
