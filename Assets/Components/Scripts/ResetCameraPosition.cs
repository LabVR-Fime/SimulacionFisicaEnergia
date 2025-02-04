using System.Collections;
using UnityEngine;

public class ResetCameraPosition : MonoBehaviour
{
    public GameObject XROrigin; // Referencia al XR Rig u objeto padre de la cámara
    public Vector3 resetPosition = Vector3.zero; // Posición deseada después de reiniciar
    public Vector3 resetRotation = Vector3.zero; // Rotación deseada después de reiniciar

    void Start()
    {
        // Si no se asignó manualmente el XR Rig, intentamos encontrarlo
        if (XROrigin == null)
        {
            XROrigin = GameObject.Find("XR Rig"); // Cambia "XR Rig" al nombre exacto de tu XR Rig en la jerarquía
        }

        // Iniciar la corrutina para reiniciar después de 0.3 segundos
        StartCoroutine(ResetCameraAfterDelay(0.3f));
    }

    IEnumerator ResetCameraAfterDelay(float delay)
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(delay);

        if (XROrigin != null)
        {
            // Reinicia la posición y rotación del XR Rig
            XROrigin.transform.position = resetPosition;
            XROrigin.transform.eulerAngles = resetRotation;

            Debug.Log("La posición y rotación de la cámara han sido reiniciadas.");
        }
        else
        {
            Debug.LogWarning("No se encontró el XR Rig. Asegúrate de asignarlo en el inspector o que el nombre sea correcto.");
        }
    }
}
