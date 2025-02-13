using System.Linq.Expressions;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float walkSlideForce;
    float slideTimer;

    public float slideYScale;
    float startYScale;

    [Header("Input")]
    public KeyCode SlideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm.sliding = false;
        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(SlideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }

        if(Input.GetKeyUp(SlideKey) && pm.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!pm.OnSlope() || rb.linearVelocity.y > -.01f)
        {
            if(pm.state == PlayerMovement.MovementState.Sprinting)
                rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            else if(pm.state == PlayerMovement.MovementState.Walking)
                rb.AddForce(inputDirection.normalized * walkSlideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        if (slideTimer < 0)
            StopSlide();
    }

    void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
