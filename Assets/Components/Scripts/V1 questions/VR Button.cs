using System.Collections;
using UnityEngine;

public class VRButton : MonoBehaviour
{
    public float deadTime = 1.0f;
    public GameObject firstObject;  // Primer objeto inicial
    public GameObject secondObject; // Primer objeto alternativo
    public GameObject thirdObject;  // Segundo objeto inicial
    public GameObject fourthObject; // Segundo objeto alternativo

    private bool _deadTimeActive = false;
    private bool _isFirstObjectActive = true; // Controla cuál objeto está activo para el primer par
    private bool _isThirdObjectActive = true; // Controla cuál objeto está activo para el segundo par

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Button") && !_deadTimeActive)
        {
            ToggleFirstPair();
            ToggleSecondPair();
            StartCoroutine(WaitForDeadTime());
            gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }

    private void ToggleFirstPair()
    {
        if (firstObject != null && secondObject != null)
        {
            // Alternar los estados del primer par de objetos
            _isFirstObjectActive = !_isFirstObjectActive;
            firstObject.SetActive(_isFirstObjectActive);
            secondObject.SetActive(!_isFirstObjectActive);

            Debug.Log($"First object is now {(firstObject.activeSelf ? "active" : "inactive")}.");
            Debug.Log($"Second object is now {(secondObject.activeSelf ? "active" : "inactive")}.");
        }
        else
        {
            Debug.LogWarning("First or second object is not assigned.");
        }
    }

    private void ToggleSecondPair()
    {
        if (thirdObject != null && fourthObject != null)
        {
            // Alternar los estados del segundo par de objetos
            _isThirdObjectActive = !_isThirdObjectActive;
            thirdObject.SetActive(_isThirdObjectActive);
            fourthObject.SetActive(!_isThirdObjectActive);

            Debug.Log($"Third object is now {(thirdObject.activeSelf ? "active" : "inactive")}.");
            Debug.Log($"Fourth object is now {(fourthObject.activeSelf ? "active" : "inactive")}.");
        }
        else
        {
            Debug.LogWarning("Third or fourth object is not assigned.");
        }
    }
}
