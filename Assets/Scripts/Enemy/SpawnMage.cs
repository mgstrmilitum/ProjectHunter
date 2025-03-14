using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;

public class SpawnMage : MonoBehaviour, TakeDamage
{
    enum EnemyType
    {
        Standard,
        Grenade,
        Melee,
        Boss1
    }

    [SerializeField] int meleeDamage;
    [SerializeField] int hp;
    [SerializeField] int maxHp;
    [SerializeField] Renderer model;
    [SerializeField] Renderer[] models;
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
    [SerializeField] float spawnRadius = 4f;
    [SerializeField] float navMeshCheckDistance = 2f;
    [SerializeField] EnemyType enemyType;
    [SerializeField] Rigidbody rb;

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
    bool spawnedSpawner = false; // Added flag so spawner is only spawned once

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        originalColor = model.material.color;
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;

        maxHp = hp;
        spawnThreshold1 = Mathf.RoundToInt(maxHp * 1 / 2f); // When HP drops below 1/2
    }

    void Update()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;

        if ((playerInRange && !CanSeePlayer()))
        {
            if (!isRoaming && agent.remainingDistance < agent.stoppingDistance)
            {
                co = StartCoroutine(Roam());
            }
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        //isRoaming = true;
        //yield return new WaitForSeconds(roamPauseTime);
        //agent.stoppingDistance = 0;
        //Vector3 randomPos = Random.insideUnitSphere * roamDistance;
        //randomPos += startingPos;
        //NavMeshHit hit;
        //NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);
        //animatorController.SetFloat("WalkSpeed", 0.5f);
        //agent.SetDestination(hit.position);
        //isRoaming = false;
        isRoaming = true;
        animatorController.SetFloat("WalkSpeed", 0f);
        yield return new WaitForSeconds(roamPauseTime);

        Vector3 randomPos = Random.insideUnitSphere * roamDistance + startingPos;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamDistance, NavMesh.AllAreas))
        {
            agent.stoppingDistance = 0f;
            animatorController.SetFloat("WalkSpeed", 0.5f);
            agent.SetDestination(hit.position);
        }
        isRoaming = false;
    }

    // Only checks if the enemy can see the player.
    bool CanSeePlayer()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(GameManager.Instance.player.transform.position);

                FaceTarget();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    //FaceTarget();
                }
                if (!isShooting && angleToPlayer <= shootFOV)
                {
                    StartCoroutine(Shoot());
                }
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
            agent.stoppingDistance = stoppingDistanceOrig;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0f;
        }
    }

    IEnumerator FlashRed()
    {
        foreach (Renderer rend in models)
        {
            rend.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer rend in models)
        {
            rend.material.color = originalColor;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        agent.isStopped = true;

        animatorController.SetTrigger("Shoot");

        yield return new WaitForSeconds(shootRate);

        agent.isStopped = false;
        isShooting = false;
    }

    void FaceTarget()
    {
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    void SpawnEnemySpawner()
    {
        if (enemySpawnerPrefab != null)
        {
            Vector3 basePos = transform.position;
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 candidatePos = basePos + randomOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePos, out hit, navMeshCheckDistance, NavMesh.AllAreas))
            {
                candidatePos = hit.position;
            }
            else
            {
                candidatePos = basePos;
            }

            Instantiate(enemySpawnerPrefab, candidatePos, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        GameManager.Instance.gameStats.shotsHit++;

        agent.SetDestination(GameManager.Instance.player.transform.position);
        animatorController.SetFloat("WalkSpeed", 1f);
        if (co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }
        agent.stoppingDistance = stoppingDistanceOrig;


        StartCoroutine(FlashRed());

        // Added condition to spawn the enemy spawner when hp falls below half (spawnThreshold1)
        if (!spawnedSpawner && hp <= spawnThreshold1 && hp > 0)
        {
            SpawnEnemySpawner();
            spawnedSpawner = true;
        }

        if (hp <= 0)
        {
            GameManager.Instance.gameStats.numKills++;
            Destroy(gameObject);
        }
    }

    public void ShootSpell()
    {
        GameObject obj = Instantiate(bullet, shootPos.position, Quaternion.identity);
        Vector3 fireDirection = GameManager.Instance.player.transform.position - shootPos.position;
        obj.GetComponent<Rigidbody>().AddForce(fireDirection * 3f, ForceMode.Impulse);
    }
}
