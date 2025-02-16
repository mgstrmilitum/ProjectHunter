using UnityEngine;

public class ProjectileShootingWeapons : MonoBehaviour
{
    public GameObject projctilePrehaber;
    private Transform projectileTransform;
    public float projectileMovingForce;
    [SerializeField] Transform shootPos;
    public int boomDamage;
    public int projctileLoad;

    EnemyAI enemy;

    GameObject projectile;
    void Start()
    {
        Setinitalreference();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            LaunchRocket();
        }
    }

    void LaunchRocket()
    {
        if (projctileLoad >0)
        {
            projectile =  Instantiate(projctilePrehaber, shootPos.position, shootPos.rotation);
            Rigidbody body = projectile.GetComponent<Rigidbody>();
            body.isKinematic=false;

            projectile.GetComponent<Rigidbody>().AddForce(projectileTransform.right *projectileMovingForce, ForceMode.Impulse);
            if (projectile.GetComponent<Rigidbody>() != null)
            {
                Destroy(projectile, 1);
            }
            
            Destroy(projectile, 22);

        }
        projctileLoad--;

    }
    void Setinitalreference()
    {
        projectileTransform= transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(projectile);
    }
}
