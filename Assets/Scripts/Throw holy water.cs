using UnityEngine;

public class Throwholywater : MonoBehaviour
{
    public Transform startPoint;
    public GameObject HolyWaterBottle;

    float range = 10;
    
    void Start()
    {

    }

    void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Launch();
        }
    }

    private void Launch()
    {
        GameObject holywaterInstance= Instantiate(HolyWaterBottle,startPoint.position,startPoint.rotation);
        holywaterInstance.GetComponent<Rigidbody>().AddForce(startPoint.forward * range, ForceMode.Impulse);
    }
}
