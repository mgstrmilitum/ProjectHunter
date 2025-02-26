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

    [Header("Gernade Audio")]
    [SerializeField] private AudioClip chargeAudio;
    [SerializeField] private AudioClip throwAudio;

    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer trajectoryLine;



    private void Start()
    {
        maincamera= Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCharging();
            }
            if (isCharging)
            {
                ChargeThrow();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Release();
            }
        }
    }

    void StartCharging()
    {
        //GernadethrowingAudio.instance.PlayOneShot(chargeAudio, 0.5f);
        
        isCharging = true;
        chargeTime=0f;

        trajectoryLine.enabled=true;
    }

    void ChargeThrow()
    {
        chargeTime+=Time.deltaTime;

        Vector3 gernadeVel= (maincamera.transform.forward+throwDirection).normalized*Mathf.Min(chargeTime*throwforce,maxForce);
        ShowTrajectory(throwPos.position+throwPos.forward, gernadeVel);
    }

    void Release()
    {
        ThrowGernade(Mathf.Min(chargeTime*throwforce, maxForce));
        isCharging= false;

        trajectoryLine.enabled=false;
    }

    void ThrowGernade(float force)
    {
        

        Vector3 spawnPos= throwPos.position+maincamera.transform.forward;//throws where we are looking

        GameObject gernade=Instantiate(gernadePrefab,spawnPos,maincamera.transform.rotation);//spawn gernade

        Rigidbody rb=gernade.GetComponent<Rigidbody>();

        Vector3 finalThrowDirction=(maincamera.transform.forward+throwDirection).normalized;

        rb.AddForce(finalThrowDirction*force,ForceMode.VelocityChange);
        //GernadethrowingAudio.instance.PlayOneShot(throwAudio, 0.5f);

        //throwing sound
    }

    //logic for showing trajectory
    void ShowTrajectory(Vector3 origin,Vector3 speed)
    {
        Vector3[] points= new Vector3[100];
        trajectoryLine.positionCount=points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            float time= i*0.1f;
            points[i]=origin+speed*time+0.5f* Physics.gravity*time*time;//Displacement Math(Inital Velocity, time, 0.5, accerlation, time^2)
        }
        trajectoryLine.SetPositions(points);
    }

}
