using System.Linq.Expressions;  
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
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
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
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
}
