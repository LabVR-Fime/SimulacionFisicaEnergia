using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class practica_colliders : MonoBehaviour
{
    public Button freezeButton;
    public Button playButton;
    public Button resetPositionButton;
    public Button slowMotionButton; // Botón de cámara lenta

    private Rigidbody rb; 
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool isPaused = false;
    private float frozenVelocity = 0f;
    private bool isSlowMotionActive = false; // Estado de cámara lenta
    public float slowMotionScale = 0.2f; // Escala de tiempo para cámara lenta

    public TextMeshProUGUI velocidadText; // Texto para mostrar la velocidad

    void Start()
    {
        if (freezeButton != null)
        {
            freezeButton.onClick.AddListener(FreezeObject);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(ResumeObject);
        }

        if (resetPositionButton != null)
        {
            resetPositionButton.onClick.AddListener(ResetObjectPosition);
        }

        if (slowMotionButton != null)
        {
            slowMotionButton.onClick.AddListener(ToggleSlowMotion);
        }
    }

    public void FreezeObject()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        isPaused = true;
        freezeButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
    }

    public void ResumeObject()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        isPaused = false;
        playButton.gameObject.SetActive(false);
        freezeButton.gameObject.SetActive(true);
    }

    public void ResetObjectPosition()
    {
        if (rb != null)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rb.velocity = Vector3.zero;
        }

        frozenVelocity = 0f;
        velocidadText.text = "0.00 m/s";

        if (isPaused)
        {
            rb.isKinematic = true;
        }
    }

    public void ToggleSlowMotion()
    {
        if (!isSlowMotionActive)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = 0.02f * slowMotionScale; // Ajusta FixedUpdate para mantener consistencia
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // Restaura FixedUpdate
        }

        isSlowMotionActive = !isSlowMotionActive;
    }
}
