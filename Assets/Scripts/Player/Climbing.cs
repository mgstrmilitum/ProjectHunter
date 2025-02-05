using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
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

    // Update is called once per frame
    void Update()
    {
        WallCheck();
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
    }

    
}
