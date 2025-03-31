using UnityEngine;
using TMPro;

public class HeightManager : MonoBehaviour
{
    public Transform movingObject; // Objeto que se mueve en la pista
    public TextMeshProUGUI heightText; // Texto para mostrar la altura

    private float realStartHeight = 6f; // 🔹 Altura inicial en metros (6 metros)
    private float unityMaxHeight = 1.6f; // 🔹 Altura máxima en Unity (1.6 metros de la pista)
    private float unityMinHeight = 0f; // 🔹 Altura mínima en Unity

    private float scaleFactor; // 🔹 Factor de escala para convertir la altura en Unity a metros reales

    void Start()
    {
        // Calcula el factor de escala entre la altura real (6m) y la altura en Unity (1.6m)
        scaleFactor = realStartHeight / unityMaxHeight;
    }

    void Update()
    {
        if (movingObject != null && heightText != null)
        {
            // Obtiene la altura en Unity
            float unityHeight = movingObject.position.y;

            // Calcula la altura real en metros usando el factor de escala
            float realHeight = unityHeight * scaleFactor;

            // Muestra la altura real en el texto con dos decimales
            heightText.text = "Altura: " + realHeight.ToString("F2") + " m";
        }
    }
}
