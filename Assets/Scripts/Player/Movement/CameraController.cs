using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!GameManager.Instance.isPaused)
        //{
            //get input
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

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
            transform.localRotation = Quaternion.Euler(rotX, 0, 0);

            //rotate the player about the y axis
            GameManager.Instance.player.transform.Rotate(Vector3.up * mouseX);
        //}
    }
}
