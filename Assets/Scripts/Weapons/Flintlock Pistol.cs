using UnityEngine;

public class FlintlockPistol : Weapon 
{
    public string gunName;

    public LayerMask targetLayerMask;

    [Header("-----Shoot Info-----")]
    public float shootingRange;
    public float fireRate;

    [Header("-----Reload Info-----")]
    public int magazineSize;
    public float timeToReload;

    private Player player;
    private int currentAmmo;
    public int Damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = weaponData.magazineSize;

        player = cameraTransform.root.GetComponent<Player>();
    }

    public override void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit fired;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out fired, shootingRange))
            {
                magazineSize--;

                Knockbackable knock = fired.collider.GetComponent<Knockbackable>();
                if (knock != null)
                {
                    knock.ConfirmKnock(transform);
                }

                IDamageable dmg = fired.collider.GetComponent<IDamageable>();
                if (dmg!=null)
                {
                    dmg.TakeDamage((int)Damage);
                }

            }
        }
    }
}
