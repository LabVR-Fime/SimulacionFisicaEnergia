using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeMassScript : MonoBehaviour
{
    public Slider scaleSlider;
    public Slider gravitySlider;
    public Slider frictionSlider;

    public PieChart pieChart;
    public GameObject pieChartObject;
    public Button toggleChartButton;

    public TextMeshProUGUI velocidadText;
    public TextMeshProUGUI alturaText;
    public GameObject objeto3D;
    public GameObject objeto3DAltura;
    public GameObject panel;
    public GameObject chartDataContainer;

    public Button toggleVisibilityButton;
    public Button freezeButton;
    public Button resetPositionButton;
    public Button playButton;
    public Button showHeightButton;
    public Button hideHeightButton;
    public Button slowMotionButton; // Botón de cámara lenta
    public Button showChartDataButton;    // Botón para mostrar los datos
    public Button hideChartDataButton;    // Botón para ocultar los datos

    public TextMeshProUGUI kineticEnergyText;
    public TextMeshProUGUI potentialEnergyText;
    public TextMeshProUGUI thermalEnergyText;

    private Rigidbody rb;
    private Collider objectCollider;

    private float kineticEnergy;
    private float potentialEnergy;
    private float thermalEnergy;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool isPaused = false;
    private float frozenVelocity = 0f;

    private bool isSlowMotionActive = false; // Estado de cámara lenta
    public float slowMotionScale = 0.2f; // Escala de tiempo para cámara lenta

    private PhysicMaterial objectPhysicMaterial; // Material físico del objeto

    public Transform movingObject; // Objeto que se mueve en la pista
    public TextMeshProUGUI heightText; // Texto para mostrar la altura

    [SerializeField] private float realMaxHeight = 5.85f; // 🔹 Máxima altura real en metros (editable)
    [SerializeField] private float realMinHeight = 0f;    // 🔹 Mínima altura real en metros (editable)

    [SerializeField] private float unityMaxHeight = 0.4f;  // 🔹 Máxima altura en Unity (editable)
    [SerializeField] private float unityMinHeight = -0.4f; // 🔹 Altura mínima en Unity (ajustado)

    private float scaleFactor; // 🔹 Factor de escala para convertir la altura en Unity a metros reales

    private float realHeight;  // Stores the calculated real height

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Crear y asignar el material físico al collider
        objectPhysicMaterial = new PhysicMaterial();
        objectCollider.material = objectPhysicMaterial;

        scaleFactor = (realMaxHeight - realMinHeight) / (unityMaxHeight - unityMinHeight);

        if (toggleChartButton != null)
        {
            toggleChartButton.onClick.AddListener(TogglePieChartVisibility);
        }

        if (toggleVisibilityButton != null)
        {
            toggleVisibilityButton.onClick.AddListener(ToggleVisibility);
        }

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

        if (showHeightButton != null)
        {
            showHeightButton.onClick.AddListener(ShowHeightText);
        }

        if (hideHeightButton != null)
        {
            hideHeightButton.onClick.AddListener(HideHeightText);
        }

        if (slowMotionButton != null)
        {
            slowMotionButton.onClick.AddListener(ToggleSlowMotion);
        }

        if (showChartDataButton != null)
        {
            showChartDataButton.onClick.AddListener(ShowChartData);
        }

        if (hideChartDataButton != null)
        {
            hideChartDataButton.onClick.AddListener(HideChartData);
        }

        // Estado inicial
        if (chartDataContainer != null)
        {
            chartDataContainer.SetActive(false); // Los datos están ocultos al inicio
        }

        if (showChartDataButton != null)
        {
            showChartDataButton.gameObject.SetActive(true);
        }

        if (hideChartDataButton != null)
        {
            hideChartDataButton.gameObject.SetActive(false);
        }

        objeto3D.SetActive(false);
        objeto3DAltura.SetActive(false);
        panel.SetActive(true);
        alturaText.gameObject.SetActive(false);
        pieChartObject.SetActive(false);
        playButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (movingObject != null && heightText != null)
    {
        // Compute height in Unity and convert to real-world meters
        float unityHeight = movingObject.position.y - unityMinHeight;
        realHeight = unityHeight * scaleFactor + realMinHeight;  // Store real height

        // Update UI text
        heightText.text = "Altura: " + realHeight.ToString("F2") + " m";
    }

    if (!isPaused)
    {
        // Control the gravity based on the slider
        if (gravitySlider != null)
        {
            // Ajusta la gravedad entre 1.0 m/s² y 26.0 m/s²
            float gravityScale = Mathf.Lerp(1.0f, 26.0f, gravitySlider.value);
            Physics.gravity = new Vector3(0, -gravityScale, 0);  // Aplica la gravedad ajustada en el eje Y
        }

        // Aplica fricción si es necesario
        if (frictionSlider != null && objectPhysicMaterial != null)
        {
            float frictionValue = Mathf.Lerp(0f, 1f, frictionSlider.value);
            objectPhysicMaterial.dynamicFriction = frictionValue;
            objectPhysicMaterial.staticFriction = frictionValue;

            // Aplicar fuerza de fricción proporcional a la velocidad
            if (rb.velocity.magnitude > 0.01f) // Evitar aplicar fuerza si está casi en reposo
            {
                Vector3 frictionForce = -rb.velocity.normalized * frictionValue * rb.mass;
                rb.AddForce(frictionForce);
            }
        }

        // Energías calculadas
        CalculateEnergies();
        UpdatePieChart();
        UpdateVelocityText();

        // Mostrar los valores en la consola
        Debug.Log("Gravedad: " + Mathf.Abs(Physics.gravity.y) + " m/s²");
        Debug.Log("Masa: " + rb.mass + " kg");
        Debug.Log("Fricción: " + Mathf.Lerp(0f, 1f, frictionSlider.value));
        Debug.Log("Altura: " + realHeight.ToString("F2") + " m");
        Debug.Log("Velocidad: " + rb.velocity.magnitude.ToString("F2") + " m/s");
    }
    }

        void CalculateEnergies()
        {
            float mass = rb.mass;  // Masa en kg
            float gravity = Mathf.Abs(Physics.gravity.y);  // Gravedad en m/s² (ahora debería cambiar con el slider)
            float height = realHeight;  // Altura en metros reales
            float velocity = rb.velocity.magnitude;  // Velocidad en m/s
            float friction = frictionSlider.value;  // Coeficiente de fricción
            float deltaTime = Time.deltaTime;  // Tiempo transcurrido en segundos

            // Energía cinética (Ec = 1/2 * m * v^2)
            kineticEnergy = 0.5f * mass * velocity * velocity;

            // Energía potencial gravitatoria (Ep = m * g * h)
            potentialEnergy = mass * gravity * height;

            // Energía térmica aproximada (Et = µ * m * g * v * t)
            thermalEnergy += friction * mass * gravity * velocity * deltaTime;
        }

    void UpdatePieChart()
    {
        if (pieChart != null)
        {
            float[] energies = new float[] { kineticEnergy, potentialEnergy, thermalEnergy };
            pieChart.SetValues(energies);
        }

        if (kineticEnergyText != null)
        {
            kineticEnergyText.text = "Cinetica:                               " + kineticEnergy.ToString("F2") + "";
        }
        if (potentialEnergyText != null)
        {
            potentialEnergyText.text = "Potencial:                               " + potentialEnergy.ToString("F2") + "";
        }
        if (thermalEnergyText != null)
        {
            thermalEnergyText.text = "Termica:                               " + thermalEnergy.ToString("F2") + "";
        }
    }

    void UpdateVelocityText()
    {
        if (velocidadText != null && rb != null)
        {
            if (isPaused)
            {
                velocidadText.text = frozenVelocity.ToString("F2") + " m/s";
            }
            else
            {
                float velocity = rb.velocity.magnitude;
                frozenVelocity = velocity;
                velocidadText.text = velocity.ToString("F2") + " m/s";
            }
        }
    }



    public void ShowHeightText()
    {
        if (alturaText != null)
        {
            alturaText.gameObject.SetActive(true);
            showHeightButton.gameObject.SetActive(false);
            hideHeightButton.gameObject.SetActive(true);

            if (objeto3DAltura != null)
            {
                objeto3DAltura.SetActive(true);
            }
        }
    }

    public void HideHeightText()
    {
        if (alturaText != null)
        {
            alturaText.gameObject.SetActive(false);
            hideHeightButton.gameObject.SetActive(false);
            showHeightButton.gameObject.SetActive(true);

            if (objeto3DAltura != null)
            {
                objeto3DAltura.SetActive(false);
            }
        }
    }

    public void ToggleVisibility()
    {
        if (objeto3D != null)
        {
            objeto3D.SetActive(!objeto3D.activeSelf);
        }

        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    public void TogglePieChartVisibility()
    {
        if (pieChartObject != null)
        {
            pieChartObject.SetActive(!pieChartObject.activeSelf);
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

    public void ShowChartData()
    {
        if (chartDataContainer != null)
        {
            chartDataContainer.SetActive(true); // Activa el contenedor de datos
        }

        if (showChartDataButton != null)
        {
            showChartDataButton.gameObject.SetActive(false); // Oculta el botón "Mostrar"
        }

        if (hideChartDataButton != null)
        {
            hideChartDataButton.gameObject.SetActive(true); // Muestra el botón "Ocultar"
        }
    }

    public void HideChartData()
    {
        if (chartDataContainer != null)
        {
            chartDataContainer.SetActive(false); // Desactiva el contenedor de datos
        }

        if (hideChartDataButton != null)
        {
            hideChartDataButton.gameObject.SetActive(false); // Oculta el botón "Ocultar"
        }

        if (showChartDataButton != null)
        {
            showChartDataButton.gameObject.SetActive(true); // Muestra el botón "Mostrar"
        }
    }
}

public class SliderValueText : MonoBehaviour
{
    public Slider slider;  // Asigna el slider manualmente en el Inspector
    public TextMeshProUGUI textComp;  // Usa TextMeshProUGUI en lugar de Text

    void Start()
    {
        if (slider != null && textComp != null)
        {
            UpdateText(slider.value);
            slider.onValueChanged.AddListener(UpdateText);
        }
    }

    void UpdateText(float val)
    {
        textComp.text = val.ToString("F2");  // Muestra el valor con 2 decimales
    }
}
