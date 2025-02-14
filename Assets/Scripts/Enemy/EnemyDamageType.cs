using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDamageType : MonoBehaviour// Inherit from MonoBehaviour
{
    public enum DamageType { Bullet, Stationary }

    [SerializeField] private DamageType type;
    [SerializeField] private Rigidbody rb;

    [SerializeField] public int damageAmount = 10;
    [SerializeField] public int speed = 10;
    [SerializeField] public float destroyTime = 5f;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }



        if (type == DamageType.Bullet)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;

            TakeDamage dmg = other.GetComponent<TakeDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damageAmount, other);
                Destroy(gameObject);
            }
        }
    
}



