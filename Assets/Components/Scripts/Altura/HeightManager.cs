using UnityEngine;
using TMPro;

public class HeightManager : MonoBehaviour
{
    public Transform movingObject; // Objeto que se mueve en la pista
    public TextMeshProUGUI heightText; // Texto para mostrar la altura

    [SerializeField] private float realMaxHeight = 5.85f; // 🔹 Máxima altura real en metros (editable)
    [SerializeField] private float realMinHeight = 0f;    // 🔹 Mínima altura real en metros (editable)

    [SerializeField] private float unityMaxHeight = 0.4f;  // 🔹 Máxima altura en Unity (editable)
    [SerializeField] private float unityMinHeight = -0.4f; // 🔹 Altura mínima en Unity (ajustado)

    private float scaleFactor; // 🔹 Factor de escala para convertir la altura en Unity a metros reales

    void Start()
    {
        // Calcula el factor de escala basado en el rango total de alturas
        scaleFactor = (realMaxHeight - realMinHeight) / (unityMaxHeight - unityMinHeight);
    }

    void Update()
    {
        if (movingObject != null && heightText != null)
        {
            // Obtiene la altura en Unity y la ajusta al nuevo sistema de referencia
            float unityHeight = movingObject.position.y - unityMinHeight;

            // Convierte a altura real en metros
            float realHeight = unityHeight * scaleFactor + realMinHeight;

            // Muestra la altura real en el texto con dos decimales
            heightText.text = "Altura: " + realHeight.ToString("F2") + " m";
        }
    }
}
