using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDamageType : MonoBehaviour// Inherit from MonoBehaviour
{
    public enum DamageType { Bullet, Stationary }

    [SerializeField] private DamageType type;
    [SerializeField] private Rigidbody rb;

    [SerializeField] public int damageAmount;
    [SerializeField] public int speed;
    [SerializeField] public float destroyTime;

    void Start()
    {
        //if (rb == null)
        //{
        //    rb = GetComponent<Rigidbody>();
        //}



        //if (type == DamageType.Bullet)
        //{
        //    rb.linearVelocity = transform.forward * speed;
        //    Destroy(gameObject, destroyTime);
        //}
    }

        private void OnTriggerEnter(Collider other)
        {
        if (other.isTrigger) return;
        Debug.Log("Bullet hit: " + other.gameObject.name + " | isTrigger: " + other.isTrigger);


        TakeDamage dmg = other.transform.gameObject.GetComponent<TakeDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    
}



