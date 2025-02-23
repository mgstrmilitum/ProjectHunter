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

        TakeDamage dmg = other.GetComponent<TakeDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }
    }

    //if IsLava is true, begins to deal damage to the player over time. Damage is dealt based on damageAmount and is spaced out according to damageDelay
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        TakeDamage dmg = other.GetComponent<TakeDamage>();

        if (dmg != null && isLava == true)
        {
            if (localDamageDelay <= 0)
            {
                dmg.takeDamage(damageAmount);
                if (isLava) GameManager.Instance.lavaOverlay.SetActive(true);
                localDamageDelay = damageDelay;
            }
            else
            {
                localDamageDelay -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.lavaOverlay.SetActive(false);
    }
}
