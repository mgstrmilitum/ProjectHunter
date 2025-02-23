using UnityEngine;

public class Throwholywater : MonoBehaviour
{
    [Header("Gernade prefab")]
    [SerializeField] private GameObject gernadePrefab;

    [Header("Gernade Settings")]
    [SerializeField] private Transform throwPos;
    [SerializeField] Vector3 throwDirection=new Vector3(0,1,0);


    [Header("Gernade Force")]
    [SerializeField] private float throwforce=10f;//the force thrown
    [SerializeField] private float maxForce=20f;//the max force that it can be thrown

    private Camera maincamera;
    private float chargeTime=0f;
    bool isCharging=false;
    

    private void Start()
    {
        maincamera= Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartCharging();
        }
        if (isCharging)
        {
            ChargeThrow();
        }
        if(Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    void StartCharging()
    {
        isCharging = true;
        chargeTime=0f;

        //TrajectoryLine
    }

    void ChargeThrow()
    {
        chargeTime+=Time.deltaTime;

        //trajectoryline Velocity
    }

    void Release()
    {
        ThrowGernade(Mathf.Min(chargeTime*throwforce, maxForce));
        isCharging= false;
    }

    void ThrowGernade(float force)
    {
        Vector3 spawnPos= throwPos.position+maincamera.transform.forward;//throws where we are looking

        GameObject gernade=Instantiate(gernadePrefab,spawnPos,maincamera.transform.rotation);//spawn gernade

        Rigidbody rb=gernade.GetComponent<Rigidbody>();

        Vector3 finalThrowDirction=(maincamera.transform.forward+throwDirection).normalized;

        rb.AddForce(finalThrowDirction*force,ForceMode.VelocityChange);

        //throwing sound
    }

    //logic for showing trajectory


}
