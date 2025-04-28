using UnityEngine;
using UnityEngine.UI;

public class RampaDinamica : MonoBehaviour
{
    [Header("Configuración")]
    public Slider sliderGravedad;
    public float gravedadMaxima = 30f;
    public float suavizadoMovimiento = 5f;
    
    [Header("Referencias")]
    public Transform puntoMasBajo; // Asigna el punto central de la rampa
    
    private Rigidbody rb;
    private float gravedadActual;
    private Vector3 direccionActual;
    private float alturaObjetivo;
    private bool necesitaAjusteInicial = true;

    [Header("Configuración de Rampa")]
    [Tooltip("Altura máxima de la rampa desde el punto más bajo")]
    public float alturaMaximaRampa = 5f; // Ahora es editable en el Inspector
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        
        sliderGravedad.minValue = 0f;
        sliderGravedad.maxValue = gravedadMaxima;
        sliderGravedad.onValueChanged.AddListener(ActualizarGravedad);
        
        // Inicializar con el valor actual del slider
        gravedadActual = sliderGravedad.value;
        alturaObjetivo = CalcularAlturaObjetivo();
        
        // Determinar dirección inicial basada en la posición
        direccionActual = (transform.position.x > puntoMasBajo.position.x) ? Vector3.left : Vector3.right;
        
        // Calcular velocidad inicial basada en altura actual
        if(gravedadActual > 0.1f)
        {
            float alturaInicial = Mathf.Max(0, transform.position.y - puntoMasBajo.position.y);
            rb.velocity = direccionActual * Mathf.Sqrt(2 * gravedadActual * alturaInicial);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void ActualizarGravedad(float nuevaGravedad)
    {
        gravedadActual = nuevaGravedad;
        alturaObjetivo = CalcularAlturaObjetivo();
        necesitaAjusteInicial = true; // Forzar reajuste al cambiar gravedad
    }

    float CalcularAlturaObjetivo()
    {
        float porcentajeGravedad = Mathf.Pow(gravedadActual / gravedadMaxima, 0.7f);
        return alturaMaximaRampa * porcentajeGravedad; 
    }

    
    void FixedUpdate()
    {
        if (gravedadActual <= 0.01f)
        {
            MoverHaciaCentro();
            return;
        }
        
        // Aplicar gravedad
        rb.AddForce(Physics.gravity.normalized * gravedadActual, ForceMode.Acceleration);
        
        // Ajuste inicial suave
        if(necesitaAjusteInicial)
        {
            AjustarVelocidadInicial();
            return;
        }
        
        ControlarMovimiento();
    }

    void AjustarVelocidadInicial()
    {
        float alturaActual = Mathf.Max(0, transform.position.y - puntoMasBajo.position.y);
        float velocidadDeseada = Mathf.Sqrt(2 * gravedadActual * Mathf.Min(alturaActual, alturaObjetivo));
        
        if(rb.velocity.magnitude > 0.1f)
        {
            direccionActual = rb.velocity.normalized;
        }
        
        Vector3 velocidadObjetivo = direccionActual * velocidadDeseada;
        rb.velocity = Vector3.Lerp(rb.velocity, velocidadObjetivo, suavizadoMovimiento * Time.fixedDeltaTime);
        
        // Comprobar si hemos alcanzado la velocidad deseada
        if(Mathf.Abs(rb.velocity.magnitude - velocidadDeseada) < 0.1f)
        {
            necesitaAjusteInicial = false;
        }
    }

    
    void ControlarMovimiento()
    {
        float energiaActual = gravedadActual * (transform.position.y - puntoMasBajo.position.y) + 0.5f * rb.velocity.sqrMagnitude;
        float energiaDeseada = gravedadActual * alturaObjetivo;
        
        float velocidadObjetivo = Mathf.Sqrt(Mathf.Max(0, 2 * (energiaDeseada - gravedadActual * (transform.position.y - puntoMasBajo.position.y))));
        
        if (rb.velocity.magnitude > 0.1f)
        {
            direccionActual = rb.velocity.normalized;
            Vector3 nuevaVelocidad = direccionActual * velocidadObjetivo;
            rb.velocity = Vector3.Lerp(rb.velocity, nuevaVelocidad, suavizadoMovimiento * Time.fixedDeltaTime);
        }
    }

    void MoverHaciaCentro()
    {
        Vector3 direccionAlCentro = (puntoMasBajo.position - transform.position).normalized;
        float distanciaAlCentro = Vector3.Distance(transform.position, puntoMasBajo.position);
        
        if (distanciaAlCentro < 0.1f)
        {
            rb.velocity = Vector3.zero;
            transform.position = puntoMasBajo.position;
            necesitaAjusteInicial = false;
        }
        else
        {
            rb.velocity = direccionAlCentro * Mathf.Min(2f, distanciaAlCentro * 2f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && rb.velocity.magnitude > 0.5f)
        {
            direccionActual = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal).normalized;
            necesitaAjusteInicial = false;
        }
    }
}