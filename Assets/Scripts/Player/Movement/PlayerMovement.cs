using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    //public float crouchSpeed;
    public float slideSpeed;
    //public float crouchYScale;
    public float sprintSpeed;
    float horizontalInput;
    float verticalInput;
    float startYScale;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f; // Allow jumping for 0.2 seconds after leaving the ground
    private float coyoteTimeCounter;

    [Header("Slide")]
    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    public bool sliding;
    [SerializeField] KeyCode SlideKey;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Climb")]
    public bool freeze;
    public bool climbing;

    [Header("Ground Check")]
    public float playerHeight;
    public float groundCheckDistance;
    public float groundDrag;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Camera Lean")]
    //public Transform leanPivot;
    public float currentLean;
    public float targetLean;
    public float leanAngle;
    public float leanDownAngle;
    public float leanSmoothing;
    public float leanVelocity;
    public KeyCode LeanLeftKey = KeyCode.B;
    public KeyCode LeanRightKey = KeyCode.N;
    //public KeyCode LeanDownKey = KeyCode.V;
    //private bool leanDown;
    private bool leanRight;
    private bool leanLeft;
    public bool leaning;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float slopeGroundCheckDistance;
    private RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    //public KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Camera Effects")]
    public cameraController playerCam;
    public float grappleFOV;

    [Header("Audio")]
    public AudioSource aud;
    public AudioClip[] audSteps;
    public float audStepsVol;
    public AudioClip[] audHurt;
    public float audHurtVol;
    public AudioClip[] audJump;
    public float audJumpVol;
    private bool isPlayingSteps;
    public AudioClip[] slideSounds;
    public float slideSoundsVol;
    public AudioClip[] wallGrabSounds;
    public float wallGrabSoundsVol;


    public MovementState state;
    public bool activeGrapple;

    private Vector3 velocityToSet;
    Vector3 moveDirection;
    [SerializeField] Rigidbody rb;

    private bool enableMovementOnNextTouch;
    public bool restricted;

    public Difficulty difficulty;

    public enum MovementState
    {
        Freeze,
        Walking,
        Sprinting,
        Crouching,
        Sliding,
        Air
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPlayingSteps = false;
        //leanDown = false;
        exitingSlope = false;
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckDistance, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag
        if (grounded && !activeGrapple)
        {

            rb.linearDamping = groundDrag;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            rb.angularDamping = 0f;
            coyoteTimeCounter -= Time.deltaTime;
        }

    }

    private void LateUpdate()
    {
        //CalculateLean();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnCollisionEnter(Collision collision)
    { 
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch=false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private void StateHandler()
    {
        //freeze
        if (freeze)
        {
            state = MovementState.Freeze;
            moveSpeed = 0f;
            rb.linearVelocity = Vector3.zero;
        }
        //sprint
        else if (grounded && Input.GetKey(SprintKey))
        {
            state = MovementState.Sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        //else if(grounded && Input.GetKey(CrouchKey))
        //{
        //    state = MovementState.Crouching;
        //    desiredMoveSpeed = crouchSpeed;
        //}

        //sliding
        else if (grounded && Input.GetKey(SlideKey))
        {
            state = MovementState.Sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.Walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0f)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
            moveSpeed = lastDesiredMoveSpeed;

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(JumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //if (Input.GetKey(CrouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //    if(!grounded)
        //        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); //re-ground player to prevent floating
        //}
        //if (Input.GetKeyUp(CrouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        //}
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        // Calculate move direction relative to where the player is facing
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // Normalize the movement direction
        if (moveDirection != Vector3.zero)
            moveDirection = moveDirection.normalized;

        // Handle slope movement
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 10f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // Regular ground movement
        if(moveDirection.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(PlaySteps());
        }
        //review cross products so this makes sense
        moveDirection = transform.forward * -horizontalInput + transform.right * verticalInput;
        moveDirection = Vector3.Cross(slopeHit.normal,-moveDirection);

        //if (OnSlope() && !exitingSlope)
        //{
        //    //cross product stuff here OR overhaul GetSlopeMoveDirection funciton
        //    rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed, ForceMode.Force);

        //    if (rb.linearVelocity.y > 0)
        //        rb.AddForce(Vector3.down, ForceMode.Force);
        //}

        if (grounded)
        { 
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        // Air movement
        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }


    private void SpeedControl()
    {
        if(activeGrapple) return;

        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVelo = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //limit velocity if needed
            if (flatVelo.magnitude > moveSpeed)
            {
                Vector3 limitedVelo = flatVelo.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVelo.x, rb.linearVelocity.y, limitedVelo.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Calculate jump direction based on input and orientation
        Vector3 jumpDirection = moveDirection;

        // Apply vertical jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Apply horizontal force based on movement direction
        if (jumpDirection != Vector3.zero)
        {
            rb.AddForce(jumpDirection * jumpForce * 0.5f, ForceMode.Impulse);
        }

        aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public void ResetRestrictions()
    {
        activeGrapple=false;

    }

    private void LeanLeft()
    {
        if(Input.GetKeyDown(LeanLeftKey))
        {
            leanLeft = true;
            leanRight = false;
        }
        if (Input.GetKeyUp(LeanLeftKey))
        {
            leanLeft = false;
            leanRight = false;
        }
    }

    private void LeanRight()
    {
        if (Input.GetKeyDown(LeanRightKey))
        {
            leanLeft = false;
            leanRight = true;
        }
        if (Input.GetKeyUp(LeanRightKey))
        {
            leanLeft = false;
            leanRight = false;
        }
    }
    
    private void LeanDown()
    {
        //if (Input.GetKey(LeanDownKey))
        //{
        //    targetLean = leanDownAngle;
        //    leanDown = true;
        //}
    }

    private void CalculateLean()
    {
        LeanLeft();
        LeanRight();

        if(leanLeft)
        {
            targetLean = leanAngle;
        }
        else if(leanRight)
        {
            targetLean = -leanAngle;
        }
        else targetLean = 0f;

        leaning = leanLeft || leanRight;

        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        //leanPivot.localRotation = Quaternion.Euler(new Vector3(0f, 0f, currentLean));
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        Vector3 startPosition = transform.position; // Correctly define it here
        velocityToSet = CalculateJumpVelocity(startPosition, targetPosition, trajectoryHeight); // Use it here
        SetVelocity();

        float travelTime = Vector3.Distance(startPosition, targetPosition) / moveSpeed;
        Invoke(nameof(ResetRestrictions), travelTime);



    }
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = velocityToSet;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + slopeGroundCheckDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0f;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        Vector3 displacement = endPoint - startPoint;
        float timeToTarget = Mathf.Sqrt(-2 * trajectoryHeight / Physics.gravity.y) +
                             Mathf.Sqrt(2 * (displacement.y - trajectoryHeight) / -Physics.gravity.y);

        Vector3 velocityXZ = new Vector3(displacement.x, 0, displacement.z) / timeToTarget;
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * trajectoryHeight);

        return velocityXZ + velocityY;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private IEnumerator PlaySteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (state != MovementState.Sprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingSteps = false;
    }

    IEnumerator FlashDamagePanel()
    {
        //GameManager.Instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        //GameManager.Instance.damagePanel.SetActive(false);
    }
}
