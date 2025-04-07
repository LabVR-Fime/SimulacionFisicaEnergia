using UnityEngine;

public class ToggleObjectsAndDisplay : MonoBehaviour
{
    public GameObject object1; // Primer objeto a alternar
    public GameObject object2; // Segundo objeto a alternar
    public Camera mainCamera;  // Cámara que alternará entre displays
    private bool isOn = false; // Estado del switch

    public void Toggle()
    {
        // Alterna el estado
        isOn = !isOn;

        // Activa/Desactiva los objetos según el estado
        object1.SetActive(isOn);
        object2.SetActive(!isOn);

        // Cambia el display de la cámara
        if (mainCamera != null)
        {
            mainCamera.targetDisplay = isOn ? 1 : 0; // Cambia entre Display 1 (índice 0) y Display 2 (índice 1)
        }
    }
}
