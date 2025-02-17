using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] Vector3 forceToApply;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (rb)
        {
            rb.AddForce(forceToApply, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (rb)
        {
            rb = null;
        }
    }
}
