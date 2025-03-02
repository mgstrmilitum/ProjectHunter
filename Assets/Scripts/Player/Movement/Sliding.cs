using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform playerObj;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerMovement pm;
    [SerializeField] AudioSource aud;
    

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    float slideTimer;

    [Header("Audio")]
    public AudioClip[] slideClips;
    public float slideClipsVol;

    public float slideYScale;
    float startYScale;

    [Header("Input")]
    public KeyCode SlideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm.sliding = false;
        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(SlideKey) && (horizontalInput != 0f || verticalInput != 0f) && pm.grounded)
        {
            StartSlide();
        }

        if(Input.GetKeyUp(SlideKey) && pm.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        if(!pm.grounded) rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
        aud.PlayOneShot(slideClips[Random.Range(0, slideClips.Length)], slideClipsVol);
    }

    void SlidingMovement()
    {
        Vector3 inputDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (!pm.OnSlope() || rb.linearVelocity.y > -.01f)
        {
            if(pm.grounded) rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            
            slideTimer -= Time.deltaTime;
        }
        else
            //rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        if (slideTimer < 0)
            StopSlide();
    }

    void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    private IEnumerator PlaySlideSound()
    {
        yield return null;
    }
}
