using UnityEngine;

public class ProjectileShootingWeapons : MonoBehaviour
{
    public GameObject projctilePrehaber;
    private Transform projectileTransform;
    public float projectileMovingForce;
    [SerializeField] Transform shootPos;
    public int Damage;
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
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        if (projctileLoad >0)
        {
            projectile =  Instantiate(projctilePrehaber, shootPos.position, shootPos.rotation);
            Rigidbody body = projectile.GetComponent<Rigidbody>();
            body.isKinematic=false;

            projectile.GetComponent<Rigidbody>().AddForce(projectileTransform.right *projectileMovingForce, ForceMode.Impulse);
            if (projectile.GetComponent<Rigidbody>() != null)
            {
                Destroy(projectile, 0.3f);
                if (projectile.GetComponent<EnemyAI>())
                {
                    enemy.takeDamage(Damage);
                }
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
