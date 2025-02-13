using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Rigidbody rb;
    public PhysicsMaterial physMat;
    public LayerMask whatIsWall;
    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;

    private bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    public float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public KeyCode GrabKey;

    // Update is called once per frame
    private void Update()
    {
        WallCheck();
        StateMachine();
        
        if(climbing) ClimbingMovement();
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (pm.grounded)
            climbTimer = maxClimbTime;
    }

    private void StartClimbing()
    {
        pm.climbing = true;
    }

    private void ClimbingMovement()
    {
        rb.linearVelocity = Vector3.zero;
        physMat.staticFriction = 1f;
        physMat.dynamicFriction = 1f;
        Debug.Log(physMat.dynamicFriction + ", " + physMat.staticFriction);
    }

    private void StopClimbing()
    {
        pm.climbing = false;
        physMat.staticFriction = 0f;
        physMat.dynamicFriction = 0f;
    }

    private void StateMachine()
    {
        if (wallFront && Input.GetKey(GrabKey) && wallLookAngle < maxWallLookAngle)
        {
            if (!pm.climbing && climbTimer > 0f) StartClimbing();

            //timer
            if (climbTimer > 0f) climbTimer -= Time.deltaTime;
            if (climbTimer < 0f) StopClimbing();

        }
        else
            if (climbing) StopClimbing();

    }
}
