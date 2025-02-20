using System;
using UnityEngine;
using UnityEngine.WSA;

public class GunMovementScript : MonoBehaviour
{
    public int CurrentMagazineSize;//the size on hand
    int MaxMagazine;//the size when reloading
    float pushbackForce;
    float pushbackRadius;
    public Transform pushbackPosistion;
    float upwardsModifier;
    bool reloading;
    bool canShoot =true;
    public float cooldownTime = 5f;
    private float lastUsedTime;
    public ParticleSystem mussleflash;
    public int bulletRange;
    public int Damage;
    public GameObject impactEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && canShoot && Time.deltaTime > lastUsedTime + cooldownTime)
        {
            lastUsedTime= Time.deltaTime;
            Launcher();
        }
    }

    public void Launcher()
    {
        
         if(CurrentMagazineSize > 0 && !reloading)
         {
            //move player in the opposite direction of where they are looking with AddExplosionForce
            
            //make a raycast shoot that makes an impact
            Shoot();

            //makes a particleEvent in front of the barrel on the flintlock
            //done in the Shoot mehod//
         }

        
        
    }

    public void Shoot()
    {
        mussleflash.Play();

        RaycastHit fired;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out fired, bulletRange))
        {
            CurrentMagazineSize--;
            
            TakeDamage dmg = fired.collider.GetComponent<TakeDamage>();
            if (dmg!=null)
            {

                dmg.takeDamage(Damage);
                GameObject ShotImpact = Instantiate(impactEffect, fired.point, Quaternion.LookRotation(fired.normal));
                Destroy(ShotImpact, 2f);

            }
        }
        
    }

    public void OnTriggerEnter(Collider other)//to move the player
    {
        other.GetComponent<Rigidbody>().AddExplosionForce(pushbackForce, pushbackPosistion.position, pushbackRadius,upwardsModifier);
        
    }

    public void Reloading()
    {
        if(Input.GetKeyDown("R"))
        {
            canShoot = false;
            reloading = true;
            CurrentMagazineSize= CurrentMagazineSize +MaxMagazine;
        }
    }
}
