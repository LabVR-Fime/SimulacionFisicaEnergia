using UnityEngine;

public class StartQuiz : MonoBehaviour
{
    public GameObject questCanvas; // Referencia al Canvas que se mostrará.
    public GameObject questObject; // Referencia al objeto 3D para el quest.
    public GameObject objectToHide; // Referencia al objeto que se ocultará al presionar un botón.



    // Asignación de preguntas y respuestas correctas
    private int[] correctAnswers = new int[] { 0, 2 }; // Índice del botón correcto por pregunta.

    void Start()
    {


        // Aseguramos el estado inicial de los objetos.
        if (questCanvas != null) questCanvas.SetActive(false); // Canvas desactivado al inicio.
        if (questObject != null) questObject.SetActive(false); // Objeto 3D desactivado al inicio.
        if (objectToHide != null) objectToHide.SetActive(true); // Objeto para desaparecer activo al inicio.
    }
}
