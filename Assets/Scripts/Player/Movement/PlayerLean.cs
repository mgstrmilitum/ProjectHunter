using UnityEngine;

public class PlayerLean : MonoBehaviour
{
    public float amount;
    public float slerpAmount;
    public PlayerMovement pm;

    public KeyCode LeanLeftKey = KeyCode.Z;
    public KeyCode LeanRightKey = KeyCode.X;

    private Quaternion initialRotation;

    public Transform orientation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, orientation.forward);
        HandleLean();
    }

    private void HandleLean()
    {
        if (Input.GetKey(LeanLeftKey))
        {
            pm.restricted = true;
            //Quaternion newRot = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + amount);
            transform.Rotate(orientation.forward, amount * Time.deltaTime * 20f);
           // transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * slerpAmount);
        }
        else if (Input.GetKey(LeanRightKey))
        {
            pm.restricted = true;
            //Quaternion newRot = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - amount);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, Time.deltaTime * slerpAmount);
            transform.Rotate(orientation.forward, -amount * Time.deltaTime * 20f);
        }
        else
        {
            //pm.state = PlayerMovement.MovementState.Walking;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * slerpAmount);
        }
    }
}
