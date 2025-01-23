using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    public int buttonIndex; // Índice del botón en la lista de botones (0, 1, 2, 3).
    public Material defaultColor; // Color predeterminado del botón.

    private QuizManager quizManager;

    void Start()
    {
        quizManager = FindObjectOfType<QuizManager>(); // Encuentra el script QuizManager en la escena.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller")) // Verifica si el controlador entra en el área de activación.
        {
            quizManager.OnButtonPressed(buttonIndex); // Llama a la función que maneja la respuesta.
        }
    }
}
