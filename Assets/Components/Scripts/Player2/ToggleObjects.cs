using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    public GameObject object1; // Primer objeto que se alterna
    public GameObject object2; // Segundo objeto que se alterna
    private bool isOn = false; // Estado actual del switch

    public void Toggle()
    {
        // Alterna el estado
        isOn = !isOn;

        // Activa/Desactiva los objetos según el estado
        object1.SetActive(isOn);
        object2.SetActive(!isOn);
    }
}
