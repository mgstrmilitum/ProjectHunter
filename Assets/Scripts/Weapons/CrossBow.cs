using System.Net.Sockets;
using UnityEngine;

public class CrossBow : Weapon
{


    [SerializeField] private GameObject crossBowBolt;
    [SerializeField] private Transform firePos;
    [SerializeField] private float fireSpeed;
    

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryShoot();
        }
    }

    public override void Shoot()
    {
        GameObject Bolt = Instantiate(crossBowBolt, firePos.position, firePos.rotation);
        Rigidbody Body = Bolt.GetComponent<Rigidbody>();
        Body.isKinematic = false;

        Bolt.GetComponent<Rigidbody>().AddForce(firePos.right * fireSpeed, ForceMode.Impulse);
        if (Bolt.GetComponent<Rigidbody>() != null)
        {
            Destroy(Bolt, 1);
        }

        Destroy(Bolt, 3);

    }
}

