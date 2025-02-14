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

    //when the player enters the collider, they take damage.
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

    //if IsLava is true, begins to deal damage to the player over time. Damage is dealt based on damageAmount and is spaced out according to damageDelay
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
