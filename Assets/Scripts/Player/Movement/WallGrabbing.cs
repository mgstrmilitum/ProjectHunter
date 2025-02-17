using System.Dynamic;
using UnityEngine;

public class WallGrabbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform cam;
    public Rigidbody rb;
    public AudioSource aud;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;
    public float minTimeOnLedge;
    public KeyCode GrabKey = KeyCode.C;

    [Header("Wall Jumping")]
    public KeyCode JumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("Exiting")]
    public bool exitingLedge;
    public float exitLedgeTime;
    private float exitLedgeTimer;

    private float timeOnLedge;
    public bool holding;

    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatisWall;

    [Header("Audio")]
    public AudioClip[] wallGrabSounds;
    public AudioClip[] wallJumpSounds;
    public float wallGrabSoundsVol;
    public float wallJumpSoundsVol;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;
    private GameObject grabPoint;

    private void Update()
    {
        
        LedgeDetection();
        SubStateMachine();
    }

    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool anyKeyPressed = horizontalInput != 0; /*|| verticalInput != 0;*/

        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;
            
            //update this functionality to deal with new button functionality
            //ie, wall grab will be broken by a toggled button OR a jump
            if(timeOnLedge > minTimeOnLedge && Input.GetKeyDown(GrabKey)) ExitWallGrab();

            if (Input.GetKeyDown(JumpKey)) WallJump();
        }
        else if(exitingLedge)
        {
            if (exitLedgeTimer > 0f) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;
        }
    }

    private void WallJump()
    {
        ExitWallGrab();

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
   
        Vector3 forceToAdd = transform.forward * ledgeJumpForwardForce * verticalInput + transform.up * ledgeJumpUpwardForce + transform.right * ledgeJumpForwardForce * horizontalInput;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
        aud.PlayOneShot(wallJumpSounds[Random.Range(0, wallJumpSounds.Length)], wallJumpSoundsVol);
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatisWall);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.point);

        //if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding && Input.GetKeyDown(GrabKey)) EnterWallGrab();

        if (Input.GetKeyDown(JumpKey)) WallJump();
    }

    private void EnterWallGrab()
    {
        holding = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;


        //aud.PlayOneShot(wallGrabSounds[Random.Range(0, wallGrabSounds.Length)], wallGrabSoundsVol);
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        grabPoint = new GameObject("GrabAnchor");
        grabPoint.transform.position = ledgeHit.point;
        transform.parent = grabPoint.transform;
        FreezeRigidbodyOnLedge();
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = ledgeHit.point - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.point);

        
        if (distanceToLedge > 1f)
        {
            if (rb.linearVelocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);

        }
        else
        {
            if (!pm.freeze) pm.freeze = true;
        }

        if(distanceToLedge > maxLedgeGrabDistance) ExitWallGrab();
    }
    public void ExitWallGrab()
    {
        transform.parent = null;
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;

        holding = false;
        pm.restricted = false;
        timeOnLedge = 0f;

        pm.freeze = false;
        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 0f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
