using UnityEngine;

public class GravityLift : MonoBehaviour
{
    [SerializeField] Vector3 JumpLiftVelocity;

    Rigidbody rb;

    float localVelocityDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(JumpLiftVelocity);
        }
    }
}
