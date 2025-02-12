using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    public float maxLookAngle = 85f;
    public LayerMask interactableLayer; // Capa para objetos interactuables (UI o interactivos)

    private Rigidbody rb;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RotateView();
        HandleInteraction(); // Agregamos interacción con la UI
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleInteraction()
    {
        // Raycast desde el centro de la pantalla
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, interactableLayer)) // Detecta elementos en la capa interactuable
        {
            if (Input.GetButtonDown("Fire1")) // Usualmente clic izquierdo o gatillo
            {
                // Si el objeto tiene un componente interactuable
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(); // Llamamos a la función Interact
                }
            }
        }
    }
}

// Interfaz que pueden usar los objetos interactuables
public interface IInteractable
{
    void Interact();
}
