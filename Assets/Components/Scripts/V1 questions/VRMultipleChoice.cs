using UnityEngine;
using UnityEngine.Events;

public class VRMultipleChoice : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText; // Texto de la pregunta (opcional si se usa UI)
        public int correctAnswerIndex; // Índice de la respuesta correcta
    }

    public GameObject[] buttons; // Botones para las respuestas
    public Color defaultColor = Color.white; // Color inicial de los botones
    public Color correctColor = Color.green; // Color para la respuesta correcta
    public Color incorrectColor = Color.red; // Color para la respuesta incorrecta
    public GameObject questionPanel; // Panel de preguntas
    public GameObject finalMessagePanel; // Mensaje final
    public Question[] questions; // Arreglo de preguntas
    public UnityEvent<bool> OnAnswerSelected; // Evento para respuesta seleccionada

    private int currentQuestionIndex = 0; // Índice de la pregunta actual
    private bool _answered = false; // Control para evitar respuestas múltiples
    private bool triggerActivated = false; // Estado del trigger

    private void Start()
    {
        if (buttons.Length != 4)
        {
            Debug.LogError("Debes asignar exactamente 4 botones.");
        }

        if (questions.Length == 0)
        {
            Debug.LogError("No se han configurado preguntas.");
        }

        finalMessagePanel.SetActive(false); // Asegurarse de que el mensaje final esté oculto
        ResetButtonColors();
        LoadQuestion();
    }

    public void ButtonPressed(int buttonIndex)
    {
        if (_answered) return; // Evitar múltiples respuestas

        _answered = true; // Marcar que ya se respondió
        bool isCorrect = buttonIndex == questions[currentQuestionIndex].correctAnswerIndex;

        // Cambiar el color del botón seleccionado
        ChangeButtonColor(buttonIndex, isCorrect ? correctColor : incorrectColor);

        Debug.Log(isCorrect ? "RESPUESTA CORRECTA" : "RESPUESTA INCORRECTA");

        OnAnswerSelected?.Invoke(isCorrect);

        if (isCorrect)
        {
            triggerActivated = true; // Activar el trigger para avanzar a la siguiente pregunta
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Intenta nuevamente.");
        }
    }

    private void LoadQuestion()
    {
        _answered = false;
        triggerActivated = false;

        if (currentQuestionIndex < questions.Length)
        {
            Debug.Log($"Pregunta: {questions[currentQuestionIndex].questionText}");
            ResetButtonColors(); // Restablecer los colores de los botones
        }
        else
        {
            ShowFinalMessage();
        }
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Length)
        {
            LoadQuestion();
        }
        else
        {
            ShowFinalMessage();
        }
    }

    private void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            Renderer renderer = button.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = defaultColor;
            }
        }
    }

    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Renderer renderer = buttons[buttonIndex].GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    private void ShowFinalMessage()
    {
        questionPanel.SetActive(false);
        finalMessagePanel.SetActive(true);
        Debug.Log("Quiz Finalizado.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerActivated && other.CompareTag("Player"))
        {
            NextQuestion();
        }
    }
}
