using UnityEngine;
using UnityEngine.UI;
public class KinematicSkater : MonoBehaviour
{
    public Transform[] pathPoints;

    [Header("Parámetros iniciales")]
    [Range(1.0f, 26.0f)] public float gravity = 9.81f;
    [Range(5f, 100f)] public float mass = 10f;
    [Range(1f, 5f)] public float speedMultiplier = 1.0f;

    public float positionOnPath = 0f;

    private float velocity = 0f;
    private float maxPosition;

    [Header("Sliders UI (opcional)")]
    public Slider gravitySlider;
    public Slider massSlider;
    public Slider speedSlider;

    void Start()
    {
        maxPosition = pathPoints.Length - 1;

        // Asignar valores iniciales a los sliders si están asignados
        if (gravitySlider != null)
        {
            gravitySlider.minValue = 1f;
            gravitySlider.maxValue = 26f;
            gravitySlider.value = gravity;

            gravitySlider.onValueChanged.AddListener((v) => gravity = v);
        }

        if (massSlider != null)
        {
            massSlider.minValue = 5f;
            massSlider.maxValue = 100f;
            massSlider.value = mass;

            massSlider.onValueChanged.AddListener((v) => mass = v);
        }

        if (speedSlider != null)
        {
            speedSlider.minValue = 1f;
            speedSlider.maxValue = 5f;
            speedSlider.value = speedMultiplier;

            speedSlider.onValueChanged.AddListener((v) => speedMultiplier = v);
        }
    }

    void Update()
    {
        int indexA = Mathf.FloorToInt(positionOnPath);
        int indexB = Mathf.CeilToInt(positionOnPath);
        float t = positionOnPath - indexA;

        if (indexA < 0) indexA = 0;
        if (indexB > maxPosition) indexB = (int)maxPosition;

        Vector3 posA = pathPoints[indexA].position;
        Vector3 posB = pathPoints[indexB].position;

        float slope = posB.y - posA.y;

        float acceleration = -gravity * slope * (mass / 50f);

        velocity += acceleration * Time.deltaTime;

        positionOnPath += velocity * Time.deltaTime * speedMultiplier;

        if (positionOnPath < 0)
        {
            positionOnPath = 0;
            velocity = -velocity;
        }
        else if (positionOnPath > maxPosition)
        {
            positionOnPath = maxPosition;
            velocity = -velocity;
        }

        transform.position = Vector3.Lerp(posA, posB, t);
    }
}