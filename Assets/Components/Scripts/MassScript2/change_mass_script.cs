using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhysicsSimulator : MonoBehaviour
{
    [Header("Configuración Física")]
    [Range(0.1f, 20f)] public float gravityStrength = 9.8f;
    [Range(0f, 0.1f)] public float frictionCoefficient = 0.001f;
    public bool perfectConservation = true;
    [Range(0.8f, 1f)] public float bounciness = 0.95f;

    [Header("Referencias UI")]
    public Slider gravitySlider;
    public Slider frictionSlider;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI kineticEnergyText;
    public TextMeshProUGUI potentialEnergyText;

    [Header("Puntos Clave")]
    public Transform startPoint;
    public Transform highestPoint;
    public Transform lowestPoint;

    private Rigidbody rb;
    private PhysicMaterial physicMat;
    private float totalEnergy;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        
        ConfigurePhysics();
        CalculateInitialEnergy();
    }

    void ConfigurePhysics()
    {
        // Configuración Rigidbody
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Configuración PhysicMaterial
        physicMat = new PhysicMaterial {
            dynamicFriction = frictionCoefficient,
            staticFriction = frictionCoefficient,
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        GetComponent<Collider>().material = physicMat;
    }

    void CalculateInitialEnergy()
    {
        float initialHeight = transform.position.y;
        totalEnergy = rb.mass * Mathf.Abs(Physics.gravity.y) * initialHeight;
    }

    void FixedUpdate()
    {
        if (perfectConservation)
        {
            EnforceEnergyConservation();
        }
        
        UpdatePhysicsParameters();
    }

    void Update()
    {
        UpdateUI();
        Debug.Log($"Gravedad: {Physics.gravity.y}");
    Debug.Log($"Velocidad: {rb.velocity.magnitude}");
    Debug.Log($"Posición Y: {transform.position.y}");
    }

    void EnforceEnergyConservation()
    {
        float currentHeight = transform.position.y;
        float targetSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * Mathf.Abs(initialPosition.y - currentHeight));
        
        if (rb.velocity.magnitude > targetSpeed * 1.05f)
        {
            rb.velocity = rb.velocity.normalized * targetSpeed;
        }
    }

    void UpdatePhysicsParameters()
    {
        // Actualiza gravedad desde slider
        if (gravitySlider != null)
        {
            Physics.gravity = new Vector3(0, -gravitySlider.value, 0);
        }

        // Actualiza fricción desde slider
        if (frictionSlider != null)
        {
            physicMat.dynamicFriction = frictionSlider.value * frictionCoefficient;
            physicMat.staticFriction = frictionSlider.value * frictionCoefficient;
        }
    }

    void UpdateUI()
    {
        // Actualiza textos
        if (velocityText != null)
        {
            velocityText.text = $"Velocidad: {rb.velocity.magnitude.ToString("F2")} m/s";
        }
        
        if (heightText != null)
        {
            heightText.text = $"Altura: {transform.position.y.ToString("F2")} m";
        }

        // Actualiza energías
        float kinetic = 0.5f * rb.mass * rb.velocity.sqrMagnitude;
        float potential = rb.mass * Mathf.Abs(Physics.gravity.y) * transform.position.y;
        
        if (kineticEnergyText != null) kineticEnergyText.text = $"Cinética: {kinetic.ToString("F2")} J";
        if (potentialEnergyText != null) potentialEnergyText.text = $"Potencial: {potential.ToString("F2")} J";
    }

    public void ResetSimulation()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        CalculateInitialEnergy();
    }
}