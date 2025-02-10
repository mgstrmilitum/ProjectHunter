using System.Dynamic;
using UnityEngine;

public class WallGrabbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;
    public float minTimeOnLedge;
    public KeyCode GrabKey = KeyCode.E;

    private float timeOnLedge;
    public bool holding;

    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatisWall;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

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
        }
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatisWall);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        //if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding && Input.GetKeyDown(GrabKey)) EnterWallGrab();
    }

    private void EnterWallGrab()
    {
        Debug.Log("In EnterWallGrab()");
        holding = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

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
    private void ExitWallGrab()
    {
        holding = false;
        pm.restricted = false;
        timeOnLedge = 0f;

        pm.freeze = false;
        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
