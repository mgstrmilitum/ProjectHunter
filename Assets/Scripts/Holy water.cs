using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holywater : MonoBehaviour
{

    private Collider[] hitColliders;
    private Collider hit;
    public float blastRadius;
    public float explosiveForce;
    public LayerMask explosionLayers;
    public ParticleSystem Boom;
    public int blastDamage;
    [SerializeField] SphereCollider KboomCollider;
    [SerializeField] LayerMask whatISEnemy;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contacts[0].point.ToString());
        Boom= Instantiate(Boom, collision.contacts[0].point, Quaternion.identity);//Boom at this exact spot!
        Destroy(Boom, 2f);

        if (collision.gameObject.GetComponent<EnemyAI>() != null)
        {
            collision.gameObject.GetComponent<EnemyAI>().takeDamage(blastDamage, hit);
            Destroy(collision.gameObject);
            KboomCollider.enabled=true;
            MeshRenderer meshrenderr = this.GetComponent<MeshRenderer>();
            meshrenderr.enabled=false;
            MeshFilter meshfilterrr = this.GetComponent<MeshFilter>();
            Destroy(meshfilterrr);

        }

    }

    void OnExplosion(Vector3 explosionPoint)
    {
        hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius, explosionLayers);
        foreach (Collider hotcol in hitColliders)
        {
            Debug.Log(hotcol.gameObject.name);
            if (hotcol.GetComponent<Rigidbody>() != null)
            {
                hotcol.GetComponent<Rigidbody>().isKinematic = false;
                hotcol.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce, explosionPoint, blastRadius, 1, ForceMode.Impulse);
                OnTriggerEnter(hotcol);

            }

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.GetComponent<EnemyAI>() != null)
        {
            other.GetComponent<EnemyAI>().takeDamage(blastDamage, hit);
            Destroy(other.gameObject);
        }


    }

    //public GameObject explsoiveEffect;
    //public float delay = 3f;

    //public float explosiveforce = 10f;
    //public float radius = 20f;

    //void Start()
    //{
    //    Invoke("Explosion", delay);
    //}

    //private void Explode()
    //{
    //    //check nearby colliders
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

    //    //Apply them a force
    //    foreach(Collider near in colliders)
    //    {
    //        Rigidbody rig = near.GetComponent<Rigidbody>();
    //        if (rig != null )
    //        {
    //            rig.AddExplosionForce(explosiveforce, transform.position, radius, 1, ForceMode.Impulse);
    //        }
    //    }

    //    Instantiate(explosiveforce,transform.position, transform.rotation);
    //    Destroy(gameObject);
    //}
}
