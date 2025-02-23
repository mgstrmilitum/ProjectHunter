using System.Collections;
using UnityEngine;

public class ProjectileShootingWeapons : Weapon
{
    public GameObject projctilePrehaber;
    private Transform projectileTransform;
    public float projectileMovingForce;
    [SerializeField] Transform shootPos;
    public int Damage;
    public int projctileLoad;
    public int MaxMagazine;
    int ammoCount;//to track the bullets
    bool isReloading;
    bool canShoot=true;
    public AudioSource AudioSource;
    //public AudioSource reloadSound;

    EnemyAI enemy;

    GameObject projectile;
    public override void Start()
    {
        base.Start();
        Setinitalreference();
       AudioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Input.GetButtonDown("Fire1") && projctileLoad>0 && !isReloading && canShoot)
        {
            Shoot();
            AudioSource.Play();

        }
        if (Input.GetButtonDown("Reload"))
        {//reloadSound = GetComponent<AudioSource>();
            StartCoroutine(Reloading());
            
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
            projctileLoad--;//a bullet was shot
            if (projectile.GetComponent<Rigidbody>() != null)
            {   
             
                Destroy(projectile, 0.3f);
                
            }
            
            Destroy(projectile, 100);

        }
        

    }
    void Setinitalreference()
    {
        projectileTransform= transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(projectile);
    }

    public IEnumerator Reloading()
    {

        
            canShoot = false;
            isReloading = true;
            yield return new WaitForSeconds(3);
            projctileLoad=  MaxMagazine;
            isReloading=false;
            canShoot= true;
        //reloadSound.Play();

    }

    
}
