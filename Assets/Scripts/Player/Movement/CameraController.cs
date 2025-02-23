using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    private Vector2 lookDirection;

    float rotX;
    private void OnEnable()
    {
        //GameManager.Instance.controls.Gameplay.Look.performed += ctx => lookDirection = ctx.ReadValue<Vector2>();
        //GameManager.Instance.controls.Gameplay.Look.canceled += ctx => lookDirection = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isPaused)
        {
            //get input
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //    float inputX = lookDirection.x * sensitivity * Time.deltaTime;
        //float inputY = lookDirection.y * sensitivity * Time.deltaTime;

        //if (invertY)
        //    rotX += inputY;
        //else
        //    rotX -= inputY;

        //rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

            //tie mouse y movement to rotX of camera
            if (invertY)
            {
                rotX += mouseY;

            }
            else
            {
                rotX -= mouseY;
            }

            //clamp camera rotation based on vert min and max
            rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);


            //rotate camera about the x axis
            //transform.localRotation = Quaternion.Euler(rotX, 0, 0);

            transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

            //rotate the player about the y axis
            GameManager.Instance.player.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
