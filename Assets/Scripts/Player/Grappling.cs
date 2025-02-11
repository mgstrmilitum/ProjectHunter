using System.Linq.Expressions;
using UnityEngine;
using System.Collections;


public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LayerMask grappableGround;
    public LineRenderer lr;
    public Rigidbody rb;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime = 0.5f;
    public float overshootYAxis;
    public float grapplePullSpeed = 100f;
    [SerializeField] private float maxGrappleSpeed = 10f; // Add this line
    [SerializeField] private float grappleSpeed = 10f;


    private bool isGrappledAtTarget = false;
    private float grappleTimer = 0f;
    private float maxGrappleTime = 3f;  // 3 seconds to stay at the grapple point


    //public float grappleStopDistance = 1.5f;
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCooldown;
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode GrappleKey = KeyCode.Mouse0;

    private bool grappling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(GrappleKey))
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(GrappleKey))
        {
            StopGrapple();
        }

        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);

            // The movement logic is now handled by the coroutine, so no need to call PullTowardsGrapplePoint here.
        }

        if (grapplingCooldownTimer > 0)
        {
            grapplingCooldownTimer -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }

    }

    private void StartGrapple()
    {

        if (grapplingCooldownTimer > 0f)
        {
            Debug.Log("Grapple on cooldown!");
            return;
        }
        grappling = true;
        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) ||
            Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappableGround))
        {
            grapplePoint = hit.point;
            Debug.Log("Grapple hit detected at: " + grapplePoint);
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            Debug.Log("No grapple hit detected!");
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        if (!grappling)
        {
            Debug.Log("ExecuteGrapple called but grappling is false!");
            return;
        }

        Debug.Log("Grapple executed, pulling towards: " + grapplePoint);

        // Start refined movement control immediately
        StartCoroutine(GrappleMovement());

    }

    public void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;
        grapplingCooldownTimer = grappleCooldown;
        lr.enabled = false;

        //StartCoroutine(SlowStop()); // Gradually stop movement
    }


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
            rb.velocity = direction * grappleSpeed;

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
            rb.velocity = Vector3.zero;
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