using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] int meleeDamage;
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] float meleeRate; // Total time for one melee attack cycle

    [Header("References")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform headPos;
    [SerializeField] Animator animatorController;
    [SerializeField] int roamDistance;

    Color originalColor;
    Vector3 playerDirection;
    Vector3 startingPos;

         
    bool playerInRange=false;
    bool isRoaming;
    bool isAttacking = false;

    Coroutine roamCoroutine;

    void Start()
    {
        originalColor = model.material.color;
        startingPos = transform.position;
        maxHp = hp;
    }

    void Update()
    {
        
        // Calculate direction from enemy's head toward the player's position.
        Vector3 loweredHeadPos = headPos.position - new Vector3(0, 0.2f, 0);
        playerDirection = GameManager.Instance.player.transform.position - loweredHeadPos;

        if (playerInRange)
        {
            // Chase the player.
            agent.SetDestination(GameManager.Instance.player.transform.position);

            // When within stopping distance, face the player and perform melee attack.
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
        }
        else
        {
            // Roam when the player is not nearby.
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                roamCoroutine = StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime);
        //agent.stoppingDistance = 0;
        Vector3 randomPos = Random.insideUnitSphere * roamDistance + startingPos;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        isRoaming = false;
    }

    void FaceTarget()
    {
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        // Trigger the melee swing animation.
        animatorController.SetTrigger("Swing");
        Debug.Log("Swing animation triggered");

        // Wait half the melee rate to simulate the wind-up time.
        yield return new WaitForSeconds(meleeRate * 0.5f);

        // Deal damage to the player.
        TakeDamage damageable = GameManager.Instance.player.GetComponentInParent<TakeDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(meleeDamage);
        }

        //Wait the remaining time before allowing the next attack.
       yield return new WaitForSeconds(meleeRate * 0.5f);
        agent.isStopped = false;
        isAttacking = false;


    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    public void takeDamage(int amount)
    {
        hp -= amount;

        // On taking damage, immediately re-chase the player.
        if (NavMesh.SamplePosition(GameManager.Instance.player.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Stop any current roaming.
        if (roamCoroutine != null)
        {
            StopCoroutine(roamCoroutine);
            isRoaming = false;
        }

        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //agent.stoppingDistance = 0;
        }
    }
}



