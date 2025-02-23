using UnityEditor;
using UnityEngine;

public class FlintlockPistol : Weapon
{
    public GameObject projctilePrehaber;
    private Transform projectileTransform;
    public float projectileMovingForce;
    public float adjustmentAngle;
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
        if (Input.GetButtonDown("Fire1"))
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

            projectile.GetComponent<Rigidbody>().AddForce(projectileTransform.forward * adjustmentAngle, ForceMode.Impulse);
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


    // public string gunName;

    // public LayerMask targetLayerMask;

    // [Header("-----Shoot Info-----")]
    // public float shootingRange;
    // public float fireRate;

    // [Header("-----Reload Info-----")]
    // public int magazineSize;
    // public float timeToReload;

    // //private Player player;
    //private int currentAmmo_;
    // public int Damage;
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    //     currentAmmo_ = weaponData.magazineSize;

    //     //player = cameraTransform.root.GetComponent<Player>();
    // }

    // public override void Shoot()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {

    //         RaycastHit fired;
    //         if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out fired, shootingRange))
    //         {
    //             magazineSize--;

    //             Knockbackable knock = fired.collider.GetComponent<Knockbackable>();
    //             if (knock != null)
    //             {
    //                 knock.ConfirmKnock(player.transform);
    //             }

    //             TakeDamage dmg = fired.collider.GetComponent<TakeDamage>();
    //             if (dmg!=null)
    //             {
    //                 dmg.takeDamage(Damage);
    //             }

    //         }
    //     }
    // }
}
