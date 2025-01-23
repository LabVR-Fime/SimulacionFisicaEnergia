using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Agregado TextMeshPro

public class QuestionLogic : MonoBehaviour
{
    public GameObject startButton;  // Botón de inicio
    public GameObject questionPanel;  // Panel de preguntas (oculto al principio)
    public TextMeshProUGUI questionText;  // Texto para la pregunta usando TextMeshPro
    public TextMeshProUGUI feedbackText;  // Texto de retroalimentación (Correcto/Incorrecto) usando TextMeshPro
    public Button[] answerButtons;  // Botones de respuesta

    private int currentQuestionIndex = 0;

    // Clase que define cada pregunta
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    // Lista de preguntas
    public Question[] questions;

    void Start()
    {
        startButton.SetActive(true);  // Mostrar el botón de inicio
        questionPanel.SetActive(false);  // Ocultar el panel de preguntas
    }

    // Función que inicia el quiz
    public void StartQuiz()
    {
        startButton.SetActive(false);
        questionPanel.SetActive(true);
        ShowQuestion(currentQuestionIndex);
    }

    // Función para mostrar la pregunta y respuestas
    void ShowQuestion(int questionIndex)
    {
        Question currentQuestion = questions[questionIndex];
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];  // Cambiado a TextMeshProUGUI
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(buttonIndex));
        }
    }

    // Función para verificar si la respuesta seleccionada es correcta
    void CheckAnswer(int selectedAnswerIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        if (selectedAnswerIndex == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "Correcto!";
        }
        else
        {
            feedbackText.text = "Incorrecto!";
        }

        currentQuestionIndex++;

        // Espera un tiempo para mostrar la siguiente pregunta o terminar el quiz
        Invoke("NextQuestion", 2f);  // Esperar 2 segundos antes de mostrar la siguiente pregunta
    }

    // Función para ir a la siguiente pregunta o terminar el quiz
    void NextQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            ShowQuestion(currentQuestionIndex);
            feedbackText.text = "";  // Limpiar el mensaje de retroalimentación
        }
        else
        {
            feedbackText.text = "¡Quiz completado!";
            // Aquí puedes añadir lógica para finalizar el quiz
        }
    }
}
