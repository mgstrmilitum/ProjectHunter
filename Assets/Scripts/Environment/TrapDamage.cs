using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] bool isLava;

    [SerializeField] int damageAmount;
    [SerializeField] float damageDelay;

    float localDamageDelay;

    void Start()
    {
        localDamageDelay = damageDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.isTrigger))
        {
            return;
        }

        IDamageable dmg = other.GetComponent<IDamageable>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamageable dmg = other.GetComponent<IDamageable>();

        if (dmg != null && isLava == true)
        {
            if (localDamageDelay <= 0)
            {
                dmg.TakeDamage(damageAmount);
                localDamageDelay = damageDelay;
            }
            else
            {
                localDamageDelay -= Time.deltaTime;
            }
        }
    }
}
