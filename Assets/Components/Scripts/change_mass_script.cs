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

    private float totalEnergy;
    [SerializeField] private bool perfectConservation = true; // Activa/desactiva conservación perfecta
    [SerializeField] private float energyTolerance = 0.05f; // 5% de tolerancia

    public List<Transform> trackPoints; // Puntos que definen la pista
    public LineRenderer trackRenderer;  

    [System.Serializable]
    public class KeyPoint
    {
        public Transform point;
        public float expectedHeight;
        public float expectedSpeed;
    }

    public List<KeyPoint> keyPoints;

    [Range(0.1f, 20f)] public float gravityStrength = 9.8f; // Gravedad terrestre estándar
    [Range(0f, 0.1f)] public float frictionCoefficient = 0.01f; // Fricción muy baja
    [Range(0.8f, 1f)] public float bounciness = 0.95f; // Casi perfectamente elástico
    public bool enableEnergyConservation = true; // Activar conservación de energía

    public List<Transform> trackKeyPoints = new List<Transform>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Crear y asignar el material físico al collider
        objectPhysicMaterial = new PhysicMaterial();
        objectCollider.material = objectPhysicMaterial;

        CalculateTotalEnergy(); 
        DrawTrack();
        CheckKeyPoints();

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

        objectPhysicMaterial.dynamicFriction = 0.05f;
        objectPhysicMaterial.staticFriction = 0.05f;
        objectPhysicMaterial.bounciness = 0.9f; // Alta elasticidad
        objectPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        objectPhysicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
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
                // Gravedad (sin cambios)
            if (gravitySlider != null)
            {
                float gravityScale = gravitySlider.value;
                Physics.gravity = new Vector3(0, -gravityScale, 0);
            }

            // Conservación de energía
            if (perfectConservation)
            {
                EnforceEnergyConservation();
            }

            // Aplica fricción si es necesario
            if (frictionSlider != null && objectPhysicMaterial != null)
            {
                float frictionValue = Mathf.Lerp(0f, 0f, frictionSlider.value);
                objectPhysicMaterial.dynamicFriction = frictionValue;
                objectPhysicMaterial.staticFriction = frictionValue;

                if (frictionValue > 0f && rb.velocity.magnitude > 0.0f)
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

    void EnforceEnergyConservation()
    {
        float mass = rb.mass;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float height = realHeight;
        float velocity = rb.velocity.magnitude;

        float currentEnergy = 0.5f * mass * velocity * velocity + mass * gravity * height;
        float energyRatio = currentEnergy / totalEnergy;

        // Si la energía actual excede la inicial (con tolerancia)
        if (energyRatio > 1f + energyTolerance)
        {
            // Calcular velocidad máxima permitida
            float maxAllowedKinetic = totalEnergy - mass * gravity * height;
            if (maxAllowedKinetic > 0)
            {
                float maxVelocity = Mathf.Sqrt(2 * maxAllowedKinetic / mass);
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    void DrawTrack()
    {
        if (trackRenderer != null && trackPoints.Count > 1)
        {
            trackRenderer.positionCount = trackPoints.Count;
            for (int i = 0; i < trackPoints.Count; i++)
            {
                trackRenderer.SetPosition(i, trackPoints[i].position);
            }
        }
    }

        void CalculateEnergies()
        {
            float mass = rb.mass;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float height = realHeight;
            float velocity = rb.velocity.magnitude;

            // Energías actuales
            kineticEnergy = 0.5f * mass * velocity * velocity;
            potentialEnergy = mass * gravity * height;
            thermalEnergy = 0f; // En un sistema ideal, no hay energía térmica

            // Forzar conservación de energía ajustando la velocidad
            if (kineticEnergy + potentialEnergy > totalEnergy * 1.05f) // 5% de tolerancia
            {
                // Si hay exceso de energía, reduce la velocidad
                float maxAllowedKinetic = totalEnergy - potentialEnergy;
                if (maxAllowedKinetic > 0)
                {
                    float maxVelocity = Mathf.Sqrt(2 * maxAllowedKinetic / mass);
                    rb.velocity = rb.velocity.normalized * Mathf.Min(velocity, maxVelocity);
                }
            }
        }

        void CalculateTotalEnergy()
        {
            float mass = rb.mass;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float initialHeight = (initialPosition.y - unityMinHeight) * scaleFactor + realMinHeight;
            
            // Energía total inicial (solo potencial, ya que parte del reposo)
            totalEnergy = mass * gravity * initialHeight;
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

    void CheckKeyPoints()
    {
        foreach (KeyPoint kp in keyPoints)
        {
            float distance = Vector3.Distance(transform.position, kp.point.position);
            if (distance < 0.1f) // Cuando pasa cerca de un punto clave
            {
                Debug.Log($"Punto clave alcanzado! " +
                        $"Altura: {realHeight.ToString("F2")} (esperada: {kp.expectedHeight}), " +
                        $"Velocidad: {rb.velocity.magnitude.ToString("F2")} (esperada: {kp.expectedSpeed})");
            }
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
            CalculateTotalEnergy();
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
