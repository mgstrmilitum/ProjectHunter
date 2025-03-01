using System;
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
    bool isReloading_;
    bool canShoot=true;
    public AudioSource AudioSource;
    bool projectileHitCollider;
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
        if (Time.timeScale != 0)
        {
            base.Update();
            if (Input.GetButtonDown("Fire1") && projctileLoad > 0 && !isReloading_ && canShoot)
            {
                Shoot();
                AudioSource.Play();

            }
            if (Input.GetButtonDown("Reload"))
            {//reloadSound = GetComponent<AudioSource>();
                StartCoroutine(Reloading());

            }
        }
    }

    public override void Shoot()
    {
        if (projctileLoad >0)
        {
            GameManager.Instance.gameStats.shotsFired++;
            projectile =  Instantiate(projctilePrehaber, shootPos.position, shootPos.rotation);
            Rigidbody body = projectile.GetComponent<Rigidbody>();
            body.isKinematic=false;
            projectile.GetComponent<Rigidbody>().AddForce(projectileTransform.right *projectileMovingForce, ForceMode.Impulse);
            projctileLoad--;//a bullet was shot
            if (projectile.GetComponent<Rigidbody>() != null)
            {   
                if(projectileHitCollider)
                {
                    Destroy(projectile);
                }
                Destroy(projectile, 2f);
                
            }
            


        }
        

    }

  

    void Setinitalreference()
    {
        projectileTransform= transform;
    }
    public void OnCollisionEnter()
    {
        projectileHitCollider=true;
        Destroy(projectile);
    }

    public IEnumerator Reloading()
    {

        
            canShoot = false;
            isReloading_ = true;
            yield return new WaitForSeconds(3);
            projctileLoad=  MaxMagazine;
            isReloading_=false;
            canShoot= true;
        //reloadSound.Play();

    }

    
}
