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
        if (grappling)
        {
            // Convert transform.position to Vector2 for 2D operations (player's current position)
            Vector2 playerPosition = (Vector2)transform.position;

            // Convert grapplePoint to Vector3 (needed for direction calculation)
            Vector3 grapplePoint3D = new Vector3(grapplePoint.x, grapplePoint.y, transform.position.z);

            // Calculate the direction to the grapple point
            Vector2 direction = (grapplePoint3D - transform.position).normalized;
            float distance = Vector2.Distance(grapplePoint, playerPosition);

            // Stop pulling if the player is close enough to the grapple point
            if (distance > 0.5f)  // Adjust this value as needed for your game
            {
                rb.velocity = direction * grappleSpeed;
            }
            else
            {
                // Stop pulling when close to the destination but stay at the point
                rb.velocity = Vector3.zero; // Stop the movement
                // The player stays at the grapple point when close enough, unless they release the button
            }
        }

        // Start the grapple when the button is pressed
        if (Input.GetKeyDown(GrappleKey))
            StartGrapple();

        // Stop grapple if button is released
        if (Input.GetKeyUp(GrappleKey))
            StopGrapple();

        // Handle cooldown timer (if needed)
        if (grapplingCooldownTimer > 0f)
            grapplingCooldownTimer -= Time.deltaTime;
    }

    void LateUpdate()
    {
        if(grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {

        if (grapplingCooldownTimer > 0f)
        {
              return;
        }
        grappling = true;
        pm.freeze = true;

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
        lr.SetPosition(1,grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;  // Unfreeze player movement

        // Ensure the player is pulled toward the grapple point
        Vector3 directionToGrapple = grapplePoint - transform.position;
        directionToGrapple.Normalize();
        rb.velocity = directionToGrapple * grapplePullSpeed;

        // If the player is close enough to the grapple point, stop grappling
        if (Vector3.Distance(transform.position, grapplePoint) < 1.5f)
        {
            StopGrapple();
        }
    }

    public void StopGrapple()
    {
        pm.freeze = false;  // Unfreeze the player
        grappling = false;  // Disable grappling
        grapplingCooldownTimer = grappleCooldown;  // Start cooldown timer
        lr.enabled = false;  // Disable line renderer
    }
}
