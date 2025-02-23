using UnityEngine;

public class ProjectileShootingWeapons : Weapon
{
    public GameObject projctilePrehaber;
    private Transform projectileTransform;
    public float projectileMovingForce;
    [SerializeField] Transform shootPos;
    public int Damage;
    public int projctileLoad;

    EnemyAI enemy;

    GameObject projectile;
    public override void Start()
    {
        base.Start();
        Setinitalreference();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Fire1") && Time.timeScale == 1)
        {
            Shoot();
        }
    }

    public override void Shoot()
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
