using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public Player player;
    public Transform cameraTransform;

    private int currentAmmo;
    private float nextTimeToFire;

    private bool isReloading = false; 




    public void Start()
    {
        currentAmmo = weaponData.magazineSize;

        player = cameraTransform.root.GetComponent<Player>();
    }

    public void TryReload()
    {
    //if the player is not reloading and does not have a full magazine then we can reload, else do nothing//
       if (!isReloading && currentAmmo > weaponData.magazineSize)
        {
            StartCoroutine(Reload());
        }

    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(weaponData.timeToReload);

        currentAmmo = weaponData.magazineSize;
        isReloading = false;

    }

   
    public abstract void Shoot();//Abstract shoot function to be implemented seperately for each weapon//

    public void HandleShoot()//HandleShoot to take care of some of the operations that will be consistent for every gun (ammo decrement,recoil, ETC.)
    {
        currentAmmo--;
        Shoot();
    }


    public void TryShoot()
    {
        if (!isReloading){ return; }

        if (currentAmmo <= 0f) { return; }

        if(Time.deltaTime >= nextTimeToFire) 
        {
        nextTimeToFire = Time.deltaTime +( 1/ weaponData.fireRate );//Time to fire is equal to the current time plus 1 divided by the fire rate so the weapon will fire bullets per second equal to the fire rate 
            HandleShoot();
        }

    }

   
}
