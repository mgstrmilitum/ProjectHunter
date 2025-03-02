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
    [SerializeField] private float maxGrappleSpeed = 10f;
    [SerializeField] private float grappleSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip grappleSound;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCooldown;
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode GrappleKey = KeyCode.Mouse1;

    private bool grappling;

    void Start()
    {
        lr.enabled = false;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
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
           
            return;
        }

        grappling = true;
        pm.activeGrapple = true;
        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) ||
            Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappableGround))
        {
            grapplePoint = hit.point;
            

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
        
        StartCoroutine(GrappleMovement());
    }

    private void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;
        pm.activeGrapple = false;
        grapplingCooldownTimer = grappleCooldown;
        lr.enabled = false;
        rb.isKinematic = false;
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
            Vector3 direction = (grapplePoint - transform.position).normalized; // Moved outside conflicting scopes

            if (!Input.GetKey(GrappleKey))
            {
                rb.linearVelocity = direction * grappleSpeed; // Reuse direction here
                break;
            }

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 strafeDirection = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;

            float currentGrappleSpeed = grappleSpeed;
            if (Input.GetKey(KeyCode.S))
            {
                currentGrappleSpeed *= 0.5f;
            }

            Vector3 finalForce = (direction * currentGrappleSpeed) + (strafeDirection * grappleSpeed * 0.5f); // Reuse direction here
            rb.linearVelocity = Vector3.ClampMagnitude(finalForce, maxGrappleSpeed);
            lastVelocity = rb.linearVelocity;

            yield return null;
        }

        rb.linearVelocity = lastVelocity;
        Destroy(grappleAnchor);
        StopGrapple();
    }
}