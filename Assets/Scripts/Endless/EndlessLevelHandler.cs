using System.Collections;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionPrefabs;

    GameObject[] sectionsPool = new GameObject[20];
    GameObject[] sections = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    [SerializeField]
    float sectionLength = 26f;

    void Start()
    {
        StartCoroutine(WaitForPlayerAndInitialize());
    }

    IEnumerator WaitForPlayerAndInitialize()
    {
        Debug.Log("Buscando jugador...");
        
        // Esperar hasta que el jugador sea creado
        while (playerCarTransform == null)
        {
            GameObject playerCarGO = GameObject.FindGameObjectWithTag("Player");
            if (playerCarGO != null)
            {
                playerCarTransform = playerCarGO.transform;
                Debug.Log($"¡Jugador encontrado!: {playerCarGO.name} en posición {playerCarTransform.position}");
            }
            else
            {
                Debug.Log("Jugador aún no encontrado...");
            }
            yield return new WaitForSeconds(0.1f);
        }

        // Ahora inicializar las secciones
        Debug.Log("Iniciando InitializeSections...");
        InitializeSections();
        StartCoroutine(UpdateLessOftenCO());
    }

    void InitializeSections()
    {
        // Debug para verificar si hay prefabs asignados
        if (sectionPrefabs == null || sectionPrefabs.Length == 0)
        {
            Debug.LogError("¡No hay section prefabs asignados en el EndlessLevelHandler!");
            return;
        }

        Debug.Log($"Inicializando {sectionPrefabs.Length} tipos de secciones...");

        int prefabIndex = 0;

        for (int i = 0; i < sectionsPool.Length; i++)
        {
            if (sectionPrefabs[prefabIndex] == null)
            {
                Debug.LogError($"Section prefab en índice {prefabIndex} es null!");
                continue;
            }

            sectionsPool[i] = Instantiate(sectionPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);
            Debug.Log($"Creada sección {i}: {sectionsPool[i].name}");

            prefabIndex++;
            if (prefabIndex >= sectionPrefabs.Length)
                prefabIndex = 0;
        }

        for (int i = 0; i < sections.Length; i++)
        {
            GameObject randomSection = GetRandomSectionFromPool();

            randomSection.transform.position = new Vector3(0f, 0f, i * sectionLength);
            randomSection.SetActive(true);

            sections[i] = randomSection;
            Debug.Log($"Sección activa {i}: {randomSection.name} en posición {randomSection.transform.position}");
        }

        Debug.Log("¡Secciones inicializadas correctamente!");
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }

    void UpdateSectionPositions()
    {
        if (playerCarTransform == null)
            return;

        // Encontrar la Z más adelantada entre las secciones activas para encadenar correctamente
        float maxZ = float.MinValue;
        for (int j = 0; j < sections.Length; j++)
        {
            if (sections[j] != null && sections[j].activeInHierarchy)
            {
                float z = sections[j].transform.position.z;
                if (z > maxZ) maxZ = z;
            }
        }

        for (int i = 0; i < sections.Length; i++)
        {
            // Reciclar cuando esta sección ya quedó suficientemente atrás del jugador
            if (playerCarTransform.position.z - sections[i].transform.position.z > sectionLength)
            {
                sections[i].SetActive(false);

                sections[i] = GetRandomSectionFromPool();
                maxZ += sectionLength;
                sections[i].transform.position = new Vector3(0f, 0f, maxZ);
                sections[i].SetActive(true);
            }
        }
    }

    GameObject GetRandomSectionFromPool()
    {
        int randomIndex = Random.Range(0, sectionsPool.Length);
        bool isNewSectionFound = false;

        while (!isNewSectionFound)
        {
            if (!sectionsPool[randomIndex].activeInHierarchy)
            {
                isNewSectionFound = true;
            }
            else
            {
                randomIndex++;
                if (randomIndex >= sectionsPool.Length)
                    randomIndex = 0;
            }
        }

        return sectionsPool[randomIndex];
    }
}
