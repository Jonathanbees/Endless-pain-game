using UnityEngine;
using Unity.Cinemachine;

public class PlayerCarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carPrefabs;

    [Header("Main Camera")]
    [SerializeField]
    CinemachineCamera virtualCamera;

    [Header("Menu")]
    [SerializeField]
    bool isMainMenu = false;

    GameObject playerCarInstance = null;
    int carIndex = 0;

    // Variable estática para mantener la selección entre escenas
    public static int selectedCarIndex = 0;

    void Awake() // Cambiar de Start() a Awake() para ejecutarse antes que UIHandler
    {
        if (isMainMenu)
        {
            // En el menú principal, usar el índice guardado
            carIndex = selectedCarIndex;
            SpawnCarForPreview();
        }
        else
        {
            // En el juego, spawner el carro seleccionado completamente funcional
            SpawnCarForGame();
        }
    }

    void Update()
    {
        if (isMainMenu && playerCarInstance != null)
        {
            // Rotar el carro en el menú para preview
            playerCarInstance.transform.Rotate(0, 20 * Time.deltaTime, 0);
        }
    }

    void SpawnCarForPreview()
    {
        if (playerCarInstance != null)
        {
            Destroy(playerCarInstance);
        }
        
        // Instanciar el carro para preview
        playerCarInstance = Instantiate(carPrefabs[carIndex]);
        
        // Configurar para preview (sin funcionalidad)
        SetupCarForPreview();
        
        // Actualizar la cámara
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerCarInstance.transform;
        }

        Debug.Log($"Carro preview spawneado: {playerCarInstance.name}");
    }

    void SpawnCarForGame()
    {
        Debug.Log($"Spawneando carro para juego: índice {selectedCarIndex}");
        
        // Instanciar el carro seleccionado para jugar
        playerCarInstance = Instantiate(carPrefabs[selectedCarIndex]);
        
        // Asegurar que tenga el tag correcto
        playerCarInstance.tag = "Player";
        
        // Actualizar la cámara
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerCarInstance.transform;
        }

        Debug.Log($"Carro de juego spawneado: {playerCarInstance.name} con tag: {playerCarInstance.tag}");
    }

    void SetupCarForPreview()
    {
        // NO asignar tag "Player" en preview para evitar conflictos
        playerCarInstance.tag = "Untagged";
        
        // Desactivar scripts de funcionalidad
        CarHandler carHandler = playerCarInstance.GetComponent<CarHandler>();
        InputHandler inputHandler = playerCarInstance.GetComponent<InputHandler>();
        AIHandler aiHandler = playerCarInstance.GetComponent<AIHandler>();
        
        if (carHandler != null) carHandler.enabled = false;
        if (inputHandler != null) inputHandler.enabled = false;
        if (aiHandler != null) aiHandler.enabled = false;
        
        // Hacer el Rigidbody kinematic para que no se mueva
        Rigidbody rb = playerCarInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void OnPreviousCarClicked()
    {
        if (!isMainMenu) return;
        
        carIndex--;
        if (carIndex < 0)
        {
            carIndex = carPrefabs.Length - 1;
        }
        
        // Guardar la selección
        selectedCarIndex = carIndex;
        
        // Actualizar el preview
        SpawnCarForPreview();
    }

    public void OnNextCarClicked()
    {
        if (!isMainMenu) return;
        
        carIndex++;
        if (carIndex >= carPrefabs.Length)
        {
            carIndex = 0;
        }
        
        // Guardar la selección
        selectedCarIndex = carIndex;
        
        // Actualizar el preview
        SpawnCarForPreview();
    }

    // Método público para obtener el carro seleccionado (opcional)
    public static GameObject GetSelectedCarPrefab()
    {
        // Este método podría ser útil si necesitas acceder al prefab desde otros scripts
        return null; // Se implementaría si es necesario
    }
}
