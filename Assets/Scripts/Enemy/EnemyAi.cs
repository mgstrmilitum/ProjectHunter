using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, TakeDamage
{
    enum EnemyType { Standard, Grenade, Melee }

    [SerializeField] int meleeDamage;
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float meleeRate;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int shootFOV;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform headPos;
    [SerializeField] Animator animatorController;
    [SerializeField] int WalkSpeedTrans;
    [SerializeField] int roamDistance;
    [SerializeField] float grenadeSpeed;
    [SerializeField] Transform playerTransform;
    [SerializeField] float damageMultiplier;
    [SerializeField] GameObject enemySpawnerPrefab;
    [SerializeField] Transform[] spawnerPositions;
    [SerializeField] EnemyType enemyType;

    float angleToPlayer;
    bool isShooting;
    bool isMelee;
    bool playerInRange;
    bool isRoaming;
    Color originalColor;
    Vector3 playerDirection;
    Vector3 startingPos;

    Coroutine co;
    int spawnThreshold1;
    int spawnThreshold2;

    void Start()
    {
        originalColor = model.material.color;
        startingPos = transform.position;

        maxHp = hp;
        spawnThreshold1 = Mathf.RoundToInt(maxHp * 2 / 3f); // When HP drops below 2/3
        spawnThreshold2 = Mathf.RoundToInt(maxHp * 1 / 3f); // When HP drops below 1/3
    }

    void Update()
    {
        // Calculate a lowered head position and update player direction and angle.
        Vector3 loweredHeadPos = headPos.position - new Vector3(0, 0.2f, 0);
        playerDirection = playerTransform.position - loweredHeadPos;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        if (playerInRange)
        {
            // Clamp the destination so the enemy never goes farther than roamDistance from its spawn.
            float distanceFromSpawn = Vector3.Distance(startingPos, playerTransform.position);
            Vector3 targetPosition;
            if (distanceFromSpawn > roamDistance)
            {
                // Calculate a destination along the line from startingPos to the player
                Vector3 direction = (playerTransform.position - startingPos).normalized;
                targetPosition = startingPos + direction * roamDistance;
            }
            else
            {
                targetPosition = playerTransform.position;
            }
            agent.SetDestination(targetPosition);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (CanSeePlayer())
            {
                if (enemyType != EnemyType.Melee && !isShooting && angleToPlayer <= shootFOV)
                {
                    StartCoroutine(Shoot());
                }
            }
            else
            {
                if (!isRoaming && agent.remainingDistance < 0.01f)
                {
                    co = StartCoroutine(Roam());
                }
            }
        }
        else
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime);
        agent.stoppingDistance = 0;
        // Instead of roaming around the fixed startingPos, roam from the enemy's current position.
        Vector3 randomPos = Random.insideUnitSphere * roamDistance + transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        isRoaming = false;
    }

    // Only checks if the enemy can see the player.
    bool CanSeePlayer()
    {
        Vector3 localLoweredHeadPos = headPos.position - new Vector3(0, 0.05f, 0);
        Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * 20f, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(localLoweredHeadPos, playerDirection.normalized, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * hit.distance, Color.red);
                return true;
            }
        }
        return false;
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
            agent.stoppingDistance = 0;
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        // Stop the agent from moving while shooting.
        agent.isStopped = true;

        GameObject obj = Instantiate(bullet, shootPos.position, transform.rotation);
        if (enemyType == EnemyType.Grenade)
        {
            obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * grenadeSpeed, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(shootRate);

        // Resume movement after shooting.
        agent.isStopped = false;
        isShooting = false;
    }

    void FaceTarget()
    {
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        hp -= amount;

        // Re-chase the player on taking damage.
        agent.SetDestination(playerTransform.position);

        if (co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        StartCoroutine(FlashRed());

        if (hp <= spawnThreshold1)
        {
            SpawnEnemySpawner();
            spawnThreshold1 = int.MinValue; // Prevent multiple spawns at this threshold.
        }

        if (hp <= spawnThreshold2)
        {
            SpawnEnemySpawner();
            spawnThreshold2 = int.MinValue;
        }

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void SpawnEnemySpawner()
    {
        if (enemySpawnerPrefab != null && spawnerPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnerPositions.Length);
            Instantiate(enemySpawnerPrefab, spawnerPositions[randomIndex].position, Quaternion.identity);
        }
    }
}
