using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Levels; // Arreglo de niveles.
    public GameObject Door; // Referencia a la puerta.
    public float DoorOpenAngleY = 90f; // Ángulo en Y al que se abrirá la puerta.
    int currentLevel;

    public void correctAnswer()
    {
        // Si no estamos en el último nivel.
        if (currentLevel + 1 != Levels.Length)
        {
            // Desactivar el nivel actual.
            Levels[currentLevel].SetActive(false);

            // Rotar la puerta en el eje Y a un ángulo fijo.
            if (Door != null)
            {
                Vector3 currentRotation = Door.transform.eulerAngles;
                Door.transform.eulerAngles = new Vector3(currentRotation.x, DoorOpenAngleY, currentRotation.z);
            }

            // Cambiar al siguiente nivel.
            currentLevel++;
            Levels[currentLevel].SetActive(true);
        }
    }

    public void previousLevel()
    {
        // Si no estamos en el primer nivel.
        if (currentLevel > 0)
        {
            // Desactivar el nivel actual.
            Levels[currentLevel].SetActive(false);

            // Regresar al nivel anterior.
            currentLevel--;
            Levels[currentLevel].SetActive(true);

            // Restaurar la rotación de la puerta si es necesario.
            if (Door != null)
            {
                Vector3 currentRotation = Door.transform.eulerAngles;
                Door.transform.eulerAngles = new Vector3(currentRotation.x, 0, currentRotation.z); // Vuelve a su posición inicial.
            }
        }
    }
}
