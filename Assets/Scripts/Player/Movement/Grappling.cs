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
    public float grappleDelayTime = 5f;
    public float overshootYAxis;
    public float grapplePullSpeed = 100f;
    [SerializeField] private float maxGrappleSpeed = 10f; // Add this line
    [SerializeField] private float grappleSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip grappleSound;  // Sound to play when grapple is initiated

    //private bool isGrappledAtTarget = false;
    //private float grappleTimer = 0f;
   /* private float maxGrappleTime = 3f;*/  // 3 seconds to stay at the grapple point

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCooldown; //time between grappling intervals
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode GrappleKey = KeyCode.Mouse1;

    private bool grappling;

    void Start()
    {
        lr.enabled = false;

        // Ensure audioSource is assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        Debug.Log("Gun tip active? " + gunTip.gameObject.activeSelf);
        if (Input.GetKeyDown(GrappleKey))
            StartGrapple();

        if (Input.GetKeyUp(GrappleKey))
            StopGrapple();

        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }

        if (grapplingCooldownTimer > 0)
        {
            grapplingCooldownTimer -= Time.deltaTime;
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

        if (grapplingCooldownTimer > 0f)
            return;

        grappling = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) || Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappableGround))
        {
            grapplePoint = hit.point;
            Debug.Log("Grapple hit detected at: " + grapplePoint);

            // Play grapple sound
            if (audioSource && grappleSound)
            {
                audioSource.PlayOneShot(grappleSound);
            }

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

        Debug.Log("Grapple executed, pulling towards: " + grapplePoint);
        StartCoroutine(GrappleMovement());
    }

    private void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;
        pm.activeGrapple = false;
        grapplingCooldownTimer = grappleCooldown;

        lr.enabled = false;
        //rb.isKinematic = false;
    }

    private IEnumerator GrappleMovement()
    {
        float stopDistance = 1.0f;
        GameObject grappleAnchor = new GameObject("GrappleAnchor");
        grappleAnchor.transform.position = grapplePoint;

        rb.isKinematic = false;
        Vector3 lastVelocity = rb.linearVelocity;

        while (Vector3.Distance(transform.position, grapplePoint) > stopDistance)
        {
            if (!Input.GetKey(GrappleKey))
            {
                break; // Exit if the player releases the grapple key early
            }

            Vector3 direction = (grapplePoint - transform.position).normalized;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 strafeDirection = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;

            float currentGrappleSpeed = grappleSpeed;
            if (Input.GetKey(KeyCode.S))
            {
                currentGrappleSpeed *= 0.5f;
            }

            Vector3 finalForce = (direction * currentGrappleSpeed) + (strafeDirection * grappleSpeed * 0.5f);

            rb.linearVelocity = Vector3.ClampMagnitude(finalForce, maxGrappleSpeed);
            lastVelocity = rb.linearVelocity;

            yield return null;
        }

        // Ensure the player keeps moving after the grapple ends
        rb.isKinematic = false;
        rb.linearVelocity = lastVelocity; // Let momentum carry the player

        Destroy(grappleAnchor);
        StopGrapple();
    }
}
