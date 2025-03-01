using System;
using System.Collections;
using UnityEngine;
using TMPro;

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
    bool canShoot = true;
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
        if (projctileLoad > 0)
        {
            GameManager.Instance.gameStats.shotsFired++;
            projectile = Instantiate(projctilePrehaber, shootPos.position, shootPos.rotation);
            Rigidbody body = projectile.GetComponent<Rigidbody>();
            body.isKinematic = false;
            projectile.GetComponent<Rigidbody>().AddForce(projectileTransform.right * projectileMovingForce, ForceMode.Impulse);
            projctileLoad--;//a bullet was shot
            if (projectile.GetComponent<Rigidbody>() != null)
            {


                Destroy(projectile, 2f);

            }

            UpdateAmmoUI();

        }


    }



    void Setinitalreference()
    {
        projectileTransform = transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(projectile);
    }

    public IEnumerator Reloading()
    {


        canShoot = false;
        isReloading_ = true;
        yield return new WaitForSeconds(3);
        projctileLoad = MaxMagazine;
        isReloading_ = false;
        canShoot = true;
        //reloadSound.Play();
        UpdateAmmoUI();
    }
    private void UpdateAmmoUI()
    {
        WeaponWheelController controller = FindObjectOfType<WeaponWheelController>();

        if (controller != null)
        {
            foreach (GameObject weaponObj in controller.WheelButtons)
            {
                WeaponWheel weaponWheel = weaponObj.GetComponent<WeaponWheel>();

                if (weaponWheel != null && weaponWheel.Id == WeaponWheelController.weaponId) 
                {
                    if (weaponWheel.ammoText != null)
                    {
                        if (weaponWheel.Id == 1 || weaponWheel.Id == 4)
                        {
                            weaponWheel.ammoText.text = "Ammo: \u221E"; 
                        }
                        else
                        {
                            weaponWheel.ammoText.text = "Ammo: " + projctileLoad.ToString(); 
                        }
                    }
                }
            }
        }
    }



    private void UpdateActiveWeaponAmmoUI(TextMeshProUGUI ammoTextUI)
    {
        if (ammoTextUI != null)
        {
            if (projctileLoad == -1) // Special case for infinite ammo
            {
                ammoTextUI.text = "Ammo: \u221E";
            }
            else
            {
                ammoTextUI.text = "Ammo: " + projctileLoad.ToString();
            }
        }
    }

}
