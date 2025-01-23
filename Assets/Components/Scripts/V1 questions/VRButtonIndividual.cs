using UnityEngine;

public class VRButtonIndividual : MonoBehaviour
{
    public int buttonIndex; // Índice de este botón
    public VRMultipleChoice controller; // Referencia al controlador

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Button"))
        {
            controller.ButtonPressed(buttonIndex);
            Debug.Log($"Botón {buttonIndex + 1} presionado.");
        }
    }
}
