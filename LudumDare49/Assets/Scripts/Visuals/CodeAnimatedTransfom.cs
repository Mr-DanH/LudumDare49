using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeAnimatedTransfom : MonoBehaviour {

    private Vector3 startingPos;

    private Quaternion startingRot;

    [SerializeField]
    public bool randomOffset = true;

    private float timeOffset = 0f;

    //public float rotTimeOffset = 0f;

    [SerializeField]
    public bool continuousRotate = false;

    [SerializeField]
    public Vector3 rotationSpeed = new Vector3(10,10,10);

    [SerializeField]
    public bool sinRotate = false;

    [SerializeField]
    public Vector3 sinRotSpeed = new Vector3(2, 2, 2);

    [SerializeField]
    public Vector3 sinRotAngle = new Vector3(45, 45, 45);

    [SerializeField]
    public bool bob = false;

    [SerializeField]
    public Vector3 bobFrequency = new Vector3(1, 2, 1);

    [SerializeField]
    public Vector3 bobAmount = new Vector3(0.2f, 0.5f, 0f);

    [SerializeField]
    private AnimationCurve bobCurve = AnimationCurve.Constant(0, 1, 0);

    private Quaternion constantRotation = Quaternion.identity;

    private float currentSinRotationTimer = 0;

    private Vector3 currentSinAngle = new Vector3(0, 0, 0);

    private Vector3 bobValues = new Vector3(0, 0, 0);

    //[SerializeField]
    //public bool linearMovementLoop = false;

    //[SerializeField]
    //public Vector3 movementSpeed = new Vector3(0, 0, 0);

    //[SerializeField]
    //public float movementTime = 1f;

    Vector3 movementAmount = new Vector3(0, 0, 0);







    void Start()
    {
        startingPos = transform.localPosition;

        startingRot = transform.localRotation;

        //rotTimeOffset = 0;

        if (randomOffset)
        {
            timeOffset = Random.Range(0f, 10);

            if (continuousRotate)
            {
                //rotTimeOffset = Random.Range(0f, 3.14f);

                constantRotation *= Quaternion.Euler(timeOffset * rotationSpeed);
            }

            if (sinRotate)
            {
                currentSinRotationTimer = timeOffset;
            }

        }
    }

    void Update()
    {

        Quaternion rotationToApply = startingRot;

        if (continuousRotate)
        {
            constantRotation *= Quaternion.Euler(Time.deltaTime * rotationSpeed);
        }

        rotationToApply *= constantRotation;

        if (sinRotate)
        {
            currentSinRotationTimer += Time.deltaTime;
        }

        currentSinAngle.x = sinRotAngle.x * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.x * 2 * Mathf.PI);
        currentSinAngle.y = sinRotAngle.y * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.y * 2 * Mathf.PI);
        currentSinAngle.z = sinRotAngle.z * Mathf.Sin(currentSinRotationTimer * sinRotSpeed.z * 2 * Mathf.PI);
        rotationToApply *= Quaternion.Euler(currentSinAngle);

        transform.localRotation = rotationToApply;

        //if (linearMovementLoop)
        //{
        //    float movementX = movementSpeed.x / movementTime;
        //    float movementY = movementSpeed.y / movementTime;
        //    float movementZ = movementSpeed.z / movementTime;

        //    movementAmount = new Vector3(movementX, movementY, movementZ);
        //}

        if (bob)
        {
            //float bobX = Mathf.Sin((Time.time + timeOffset) * bobFrequency.x + Mathf.PI * 0.5f);

            float xBobs = 0;
            float yBobs = 0;
            float zBobs = 0;

            if(bobFrequency.x != 0)
            {
                xBobs = (Time.time + timeOffset) / bobFrequency.x;
            }
            if(bobFrequency.y != 0)
            {
                yBobs = (Time.time + timeOffset) / bobFrequency.y;
            }
            if(bobFrequency.z != 0)
            {
                zBobs = (Time.time + timeOffset) / bobFrequency.z;
            }

            float currentXBobProportion = xBobs - Mathf.Floor(xBobs);
            float currentYBobProportion = yBobs - Mathf.Floor(yBobs);
            float currentZBobProportion = zBobs - Mathf.Floor(zBobs);
            float bobX = bobCurve.Evaluate(currentXBobProportion);
            float bobY = bobCurve.Evaluate(currentYBobProportion);
            float bobZ = bobCurve.Evaluate(currentZBobProportion);

            //float bobZ = Mathf.Sin((Time.time + timeOffset) * bobFrequency.z);

            bobValues = new Vector3(bobX, bobY, bobZ);

        }

        transform.localPosition = startingPos + movementAmount + Vector3.Scale(bobValues, bobAmount);
    }
}
