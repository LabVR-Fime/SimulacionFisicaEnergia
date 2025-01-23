using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public GameObject[] buttons; // Lista de botones A, B, C, D.
    public Material correctColor; // Color verde para respuesta correcta.
    public Material incorrectColor; // Color rojo para respuesta incorrecta.
    
    private int correctButtonIndex; // El índice del botón correcto en el array.
    private int currentQuestion = 0; // Para llevar el control de la pregunta actual.

    // Asignación de preguntas y respuestas correctas
    private int[] correctAnswers = new int[] { 0, 2 }; // Aquí defines el índice del botón correcto por pregunta.

    void Start()
    {
        SetQuestion(currentQuestion);
    }

    // Asigna la lógica para la pregunta y las respuestas correctas
    void SetQuestion(int questionIndex)
    {
        // Aquí se puede actualizar el orden de los botones o cualquier otro cambio específico para la pregunta.
        correctButtonIndex = correctAnswers[questionIndex]; // Establece el índice del botón correcto.
        
        // Aquí, puedes agregar lógica para cambiar el texto o las etiquetas de los botones según la pregunta.
    }

    // Llamar a esta función cuando un botón es presionado.
    public void OnButtonPressed(int buttonIndex)
    {
        if (buttonIndex == correctButtonIndex)
        {
            buttons[buttonIndex].GetComponent<Renderer>().material = correctColor; // Cambia el color a verde.
            Invoke("NextQuestion", 1f); // Avanza a la siguiente pregunta después de un breve retraso.
        }
        else
        {
            buttons[buttonIndex].GetComponent<Renderer>().material = incorrectColor; // Cambia el color a rojo.
        }
    }

    // Cambia a la siguiente pregunta.
    void NextQuestion()
    {
        currentQuestion++;
        if (currentQuestion < correctAnswers.Length)
        {
            SetQuestion(currentQuestion); // Asigna la nueva pregunta.
            ResetButtonColors(); // Restaura el color de los botones para la nueva pregunta.
        }
        else
        {
            // Lógica de finalización del quiz.
            Debug.Log("Quiz completado.");
        }
    }

    // Restablece los colores de los botones.
    void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            button.GetComponent<Renderer>().material = button.GetComponent<AnswerButton>().defaultColor;
        }
    }
}
