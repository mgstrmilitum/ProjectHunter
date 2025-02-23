using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public abstract class Weapon : MonoBehaviour
{
  
    public Player player;
    public Transform cameraTransform;
    [SerializeField] public int magazineSize;
    [SerializeField] private float timeToReload;
    [SerializeField] private float fireRate;

    [Header("Sway Settings")]
    [SerializeField] private float smooth = 6f;
    [SerializeField] private float swayMultiplier = .5f;
    private float maxSwayAmount = 20f;
    private Quaternion initialRotation;


    private int currentAmmo;
    private float nextTimeToFire;

    private bool isReloading = false; 




    public virtual void Start()
    {
        initialRotation = transform.localRotation;



        player = GameManager.Instance.playerScript;
    }
    
    public virtual void Update()
    {

        Sway();
    }

    public void TryReload()
    {
    //if the player is not reloading and does not have a full magazine then we can reload, else do nothing//
       if (!isReloading && currentAmmo > magazineSize)
        {
            StartCoroutine(Reload());
        }

    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(timeToReload);

        currentAmmo = magazineSize;
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
        if (isReloading){ return; }

        if (currentAmmo <= 0f) { return; }

        if(Time.time >= nextTimeToFire) 
        {
        nextTimeToFire = Time.time + ( 1/ fireRate );//Time to fire is equal to the current time plus 1 divided by the fire rate so the weapon will fire bullets per second equal to the fire rate 
            HandleShoot();
        }

    }

    public void Sway()
    {
        //Getting mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        //Calculate target rotation 
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = initialRotation * rotationX * rotationY;

      
        //Rotate the gameobject based on the gathered inputs //Slerp is basically lerp for rotations  
       transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

}
