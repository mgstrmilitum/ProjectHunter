using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] Vector3 targetPos;
    [SerializeField] Vector3 targetRot;
    [SerializeField] Vector3 targetSca;
    [SerializeField] float lengthTesting;
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isRotating;
    [SerializeField] public bool isGrowing;
    Vector3 pointA;
    Vector3 pointB;


    float min;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointA = transform.position;
        pointB = targetPos;
    }

    // Update is called once per frame
    void Update()
    {

        //moves the object back and forth
        if (isMoving)
        {
            float time = Mathf.PingPong(Time.time * speed, lengthTesting);
            transform.position = Vector3.Lerp(pointA, pointB, time);
        }

        //if Rotating is true, rotates the object constantly by the angle of rotation
        if (isRotating)
        {
            transform.Rotate(targetRot.x * Time.deltaTime, targetRot.y * Time.deltaTime, targetRot.z * Time.deltaTime);
        }

        if (isGrowing)
        {
            transform.localScale += targetSca * Time.deltaTime;
        }
    }
}

