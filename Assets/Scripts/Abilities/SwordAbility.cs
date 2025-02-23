using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;

public class SwordAbility : Ability
{
    public Player player;
    public GameObject sword;

    public Transform playerRotation;
    public Transform attackPoint;
    public float attackRange;
    public int damageAmount = 10;

    public LayerMask enemylayer = default;
    public float attackDistance;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;
    public Vector3 hitposition;


    public void Start()
    {
        player = GameManager.Instance.playerScript;

        
        
    }

    public override void Activate(GameObject parent)
    {
        if (player.currentAp == player.maxAp)
        {
            player.ResetAp();
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("Attack");
            Debug.Log("you have attacked");

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);

            foreach (Collider collider in hitEnemies)
            {
                //checking if the object that was collided with is Damageable
                TakeDamage enemy = collider.gameObject.GetComponent<TakeDamage>();

                if (enemy != null)
                {

                    enemy.takeDamage(damageAmount);
                    
                    Debug.Log("Enemy Hit");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position,attackRange);
    }

}
