using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] Transform targetPos;
    [SerializeField] float lengthTesting;
    [SerializeField] int angleOfRotation;
    [SerializeField] bool isRotating;
    Vector3 pointA;
    Vector3 pointB;


    float min;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointA = transform.position;
        pointB = targetPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Mathf.PingPong(Time.time * speed, lengthTesting);
        transform.position = Vector3.Lerp(pointA, pointB, time);
        if (isRotating )
        {
            transform.Rotate(0, 0, angleOfRotation * Time.deltaTime);
        }
    }
}

