using UnityEngine;

public class Throwholywater : MonoBehaviour
{
    public Transform startPoint;
    public GameObject HolyWaterBottle;
    private Collider[] hitColliders;
    float range = 10;
    
    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Launch();
        }
    }

    private void Launch()
    {
        
        GameObject holywaterInstance= Instantiate(HolyWaterBottle,startPoint.position,startPoint.rotation);
        holywaterInstance.GetComponent<Rigidbody>().isKinematic = false;
        holywaterInstance.GetComponent<Rigidbody>().AddForce(startPoint.forward * range, ForceMode.Impulse);
        if(holywaterInstance.GetComponent<Rigidbody>()!=null )
        {Destroy(gameObject);

        }
        
    }
}
