using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, TakeDamage
{
    enum EnemyType
    {
        Standard,
        Grenade,
        Melee
    }

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
    float stoppingDistanceOrig;
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
        stoppingDistanceOrig = agent.stoppingDistance;
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
            // Always set destination toward the player.
            agent.SetDestination(playerTransform.position);

            // If we have reached the stopping distance, face the target.
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();

                //if (enemyType == EnemyType.Melee && !isMelee)
                //{
                //    StartCoroutine(MeleeAttack());
                //}
            }

            // Check if the enemy can see the player.
            if (CanSeePlayer())
            {
               // animatorController.SetBool("Walk", false);

                // For non-melee enemies, trigger shooting if within shooting FOV.
                if (enemyType != EnemyType.Melee && !isShooting && angleToPlayer <= shootFOV)
                {
                    StartCoroutine(Shoot());
                }
            }
            else
            {
                //animatorController.SetBool("Walk", true);
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
        Vector3 randomPos = Random.insideUnitSphere * roamDistance + startingPos;
        NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamDistance, 1);
        agent.SetDestination(hit.position);
        isRoaming = false;
    }

    // Only checks if the enemy can see the player.
    bool CanSeePlayer()
    {
        Vector3 localLoweredHeadPos = headPos.position - new Vector3(0, 0.05f, 0);
        //Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * 20f, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(localLoweredHeadPos, playerDirection.normalized, out hit))
        {
            Debug.Log("Ray hit: " + hit.collider.name); // Log what the ray is hitting

            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                //Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * hit.distance, Color.red); // Show hit in red
                //Debug.Log("Player detected! Angle: " + angleToPlayer + "°");

                return true;
            }
            else
            {
                Debug.Log("Ray did not hit player. Hit: " + hit.collider.tag);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
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
        // Stop the agent from moving while shooting
        agent.isStopped = true;

        GameObject obj = Instantiate(bullet, shootPos.position, transform.rotation);
        if (enemyType == EnemyType.Grenade)
        {
            obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * grenadeSpeed, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(shootRate);

        // Resume agent movement after shooting
        agent.isStopped = false;
        isShooting = false;
    }

    //IEnumerator MeleeAttack()
    //{
    //    isMelee = true;

    //    // Trigger the swing animation
    //    animatorController.SetTrigger("Swing");
    //    Debug.Log("Swing animation triggered");

    //    // Wait for the duration of the melee animation before applying damage
    //    yield return new WaitForSeconds(meleeRate * 0.5f); // Adjust timing if necessary

    //    // Apply damage after the animation has started
    //    TakeDamage damageable = playerTransform.GetComponent<TakeDamage>();
    //    if (damageable != null)
    //    {
    //        Collider playerCollider = playerTransform.GetComponent<Collider>();
    //        damageable.takeDamage(meleeDamage, playerCollider);
    //    }

    //    // Wait for the full melee cooldown before allowing another attack
    //    yield return new WaitForSeconds(meleeRate * 0.5f);

    //    isMelee = false;
    //}


    void FaceTarget()
    {
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        // if (hitCollider.CompareTag("Head"))
        //{
        //     hp -= Mathf.RoundToInt(amount * damageMultiplier);
        // }
        // else if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Body"))
        // {
        //     hp -= Mathf.RoundToInt(amount);
        // }
        hp -= amount;

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
            spawnThreshold1 = int.MinValue; // Prevent multiple spawns at this threshold
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
