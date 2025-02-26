using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLean : MonoBehaviour
{
    public float amount;
    public float slerpAmount;
    public PlayerMovement pm;
    float currentAngle;

    public KeyCode LeanLeftKey = KeyCode.Q;
    public KeyCode LeanRightKey = KeyCode.E;

    private Quaternion initialRotation;
    private Quaternion leftRotation;
    private Quaternion rightRotation;

    private bool hasTitledInAir;

    private void Awake()
    {
        //GameManager.Instance.controls.Gameplay.LeanLeft.performed += ctx => leanLeft = true;
        //GameManager.Instance.controls.Gameplay.LeanRight.performed += ctx => leanRight = true;
        //GameManager.Instance.controls.Gameplay.LeanLeft.canceled += ctx => leanLeft = false;
        //GameManager.Instance.controls.Gameplay.LeanRight.canceled += ctx => leanRight = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.localRotation;
        leftRotation = Quaternion.Euler(0f, 0f, amount);
        rightRotation = Quaternion.Euler(0f, 0f, -amount);
    }

    // Update is called once per frame
    void Update()
    {
        HandleLean();
    }

    private void HandleLean()
    {
        if (pm.grounded || !hasTitledInAir)
        {
            if (Input.GetKey(LeanLeftKey))
            {
                pm.restricted = true;
                Quaternion newRot = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + amount);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * slerpAmount);
            }
            else if (Input.GetKey(LeanRightKey))
            {
                pm.restricted = true;
                Quaternion newRot = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - amount);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * slerpAmount);
            }
            else
            {
                pm.restricted = false;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * slerpAmount);
            }
        }
    }
}
