using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    TextMeshProUGUI distanceTraveledText;

    [Header("Game References")]
    [SerializeField]
    CarHandler playerCar;

    void Start()
    {
        // Buscar el texto dentro del Panel
        if (distanceTraveledText == null)
        {
            GameObject panel = GameObject.Find("Canvas/Distance Canvas/Panel");
            if (panel != null)
            {
                distanceTraveledText = panel.GetComponentInChildren<TextMeshProUGUI>();
                if (distanceTraveledText != null)
                {
                    Debug.Log("UI Text encontrado automáticamente: " + distanceTraveledText.name);
                }
            }
        }

        FindPlayerCar();
    }

    void FindPlayerCar()
    {
        if (playerCar == null)
        {
            GameObject playerCarGO = GameObject.FindGameObjectWithTag("Player");
            if (playerCarGO != null)
            {
                playerCar = playerCarGO.GetComponent<CarHandler>();
                Debug.Log("Player Car encontrado automáticamente: " + playerCarGO.name);
            }
        }
    }

    void Update()
    {
        // Re-buscar el jugador si se perdió la referencia
        if (playerCar == null)
        {
            FindPlayerCar();
        }

        if (playerCar != null && distanceTraveledText != null)
        {
            distanceTraveledText.text = playerCar.DistanceTraveled.ToString("000000");
        }
    }
}
