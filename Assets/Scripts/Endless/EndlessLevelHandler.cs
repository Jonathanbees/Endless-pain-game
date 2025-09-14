using System.Collections;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionPrefabs;

    GameObject[] sectionsPool = new GameObject[20]; // all sections 

    GameObject[] sections = new GameObject[10]; // visible sectios

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    [SerializeField]
    float sectionLength = 26f;

    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        for (int i = 0; i < sectionsPool.Length; i++)
        {
            sectionsPool[i] = Instantiate(sectionPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);

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
        }
        
        StartCoroutine(UpdateLessOftenCO());
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
