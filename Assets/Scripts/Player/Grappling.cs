using System.Linq.Expressions;  
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LayerMask grappableGround;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCooldown; //time between grappling intervals
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode GrappleKey = KeyCode.Mouse1;

    private bool grappling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(GrappleKey))
            StartGrapple();

        if(Input.GetKeyUp(GrappleKey))
            StopGrapple();

        if(grapplingCooldownTimer > 0f)
            grapplingCooldownTimer -= Time.deltaTime;
    } 

    void LateUpdate()
    {
        if(grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        pm.freeze = true;

        if (grapplingCooldownTimer > 0f)
            return;

        grappling = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) || Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappableGround))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if(grapplePointRelativeYPos < 0f) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 2f);
    }

    private void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;
        pm.activeGrapple = false;
        grapplingCooldownTimer = grappleCooldown;

        lr.enabled = false;
    }
<<<<<<< HEAD


    //private void PullTowardsGrapplePoint()
    //{
    //    if (!isGrappledAtTarget)
    //    {
    //        Vector3 direction = (grapplePoint - transform.position).normalized;
    //        float distance = Vector3.Distance(transform.position, grapplePoint);

    //        // Use force-based movement instead of direct position setting
    //        rb.AddForce(direction * grapplePullSpeed, ForceMode.Acceleration);
    //        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxGrappleSpeed);

    //        // If close enough, consider stopping
    //        if (distance < 1.2f)
    //        {
    //            isGrappledAtTarget = true;
    //            rb.velocity = Vector3.zero;
    //            rb.isKinematic = true;
    //            transform.position = grapplePoint; // Only snap if very close
    //        }
    //    }
    //}

    private IEnumerator GrappleMovement()
    {
        float stopDistance = 1.0f; // Distance before stopping
        float maxGrappleDuration = 1.5f; // Max time allowed to reach the point
        float grappleStartTime = Time.time;

        GameObject grappleAnchor = new GameObject("GrappleAnchor");
        grappleAnchor.transform.position = grapplePoint;

        bool hasReachedPoint = false;

        while (Time.time - grappleStartTime < maxGrappleDuration)
        {
            Vector3 direction = (grapplePoint - transform.position).normalized;
            rb.linearVelocity = direction * grappleSpeed;

            if (Vector3.Distance(transform.position, grapplePoint) <= stopDistance)
            {
                hasReachedPoint = true;
                break;
            }

            yield return null;
        }

        if (hasReachedPoint)
        {
            // The player reached the point and should anchor
            rb.linearVelocity = Vector3.zero;
            transform.parent = grappleAnchor.transform;
            rb.isKinematic = true;

            // Check for ceilings and adjust positioning
            RaycastHit ceilingCheck;
            if (Physics.Raycast(transform.position, Vector3.up, out ceilingCheck, 1.5f, whatIsGrappleable))
            {
                transform.position = ceilingCheck.point - Vector3.up * 0.5f;
            }

            yield return new WaitForSeconds(maxGrappleTime);
        }

        // Cleanup regardless of reaching or not
        transform.parent = null;
        rb.isKinematic = false;
        Destroy(grappleAnchor);
        StopGrapple();
    }

    //private IEnumerator SlowStop()
    //{
    //    while (rb)
    //    rb.velocity = Vector3.zero; // Fully stop at the end
    //}




}
=======
}
>>>>>>> parent of ecf7c9b (Merging w dev)
