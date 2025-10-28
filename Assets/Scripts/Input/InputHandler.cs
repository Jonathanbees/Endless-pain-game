using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    private void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }

        // Auto-resolver referencia si no está asignada por prefab
        if (carHandler == null)
        {
            carHandler = GetComponent<CarHandler>();
        }
    }

    void Update()
    {
        if (carHandler == null)
        {
            // Intento tardío único por si el orden de inicialización cambió
            carHandler = GetComponent<CarHandler>();
            if (carHandler == null) return;
        }
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        carHandler.SetInputVector(inputVector);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
