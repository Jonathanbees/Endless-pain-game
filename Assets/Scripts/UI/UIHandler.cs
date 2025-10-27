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

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (playerCar == null)
        {
            GameObject playerCarGO = GameObject.FindGameObjectWithTag("Player");
            if (playerCarGO != null)
            {
                playerCar = playerCarGO.GetComponent<CarHandler>();
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCar != null && distanceTraveledText != null)
        {
            distanceTraveledText.text = playerCar.DistanceTraveled.ToString("000000");
        }
    }
}
