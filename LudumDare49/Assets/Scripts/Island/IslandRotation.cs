using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IslandRotation : MonoBehaviour
{
    private Quaternion startingRot;

    [SerializeField]
    public bool continuousRotate = false;

    [SerializeField]
    public Vector3 rotationSpeed = new Vector3(10,10,10);
    [SerializeField]
    public Vector3 sinRotAngle = new Vector3(45, 45, 45);
    [SerializeField]
    public Vector3 sinRotSpeed = new Vector3(2, 2, 2);

    private Quaternion constantRotation = Quaternion.identity;
    private float currentSinRotationTimer = 0;

    private Vector3 currentSinAngle = new Vector3(0, 0, 0);

    float timeOffset = 0f;

    Vector3 draggingSpeed;

    void Start()
    {
        startingRot = transform.localRotation;

        timeOffset = Random.Range(0f, 10);

        if (continuousRotate)
        {
            constantRotation *= Quaternion.Euler(timeOffset * rotationSpeed);
        }

        DragCatcher.onDragDelta += OnDrag;
    }

    void OnDrag(Vector2 dragDelta)
    {
        draggingSpeed = new Vector3(0, dragDelta.x, 0);
    }

    void Update()
    {
        Quaternion rotationToApply = startingRot;

        if (DragCatcher.Instance.IsDragging)
        {
            constantRotation *= Quaternion.Euler(draggingSpeed);
            draggingSpeed = Vector3.zero;
        }
        else if (continuousRotate)
        {
            constantRotation *= Quaternion.Euler(Time.deltaTime * rotationSpeed);
        }

        rotationToApply *= constantRotation;

        currentSinAngle.x = sinRotAngle.x * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.x * 2 * Mathf.PI);
        currentSinAngle.y = sinRotAngle.y * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.y * 2 * Mathf.PI);
        currentSinAngle.z = sinRotAngle.z * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.z * 2 * Mathf.PI);
        rotationToApply *= Quaternion.Euler(currentSinAngle);

        transform.localRotation = rotationToApply;
    }
}
