using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBossAi : MonoBehaviour, TakeDamage
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
    [SerializeField] Transform headPos;
    [SerializeField] Animator animatorController;
    [SerializeField] int WalkSpeedTrans;
    [SerializeField] float grenadeSpeed;
    [SerializeField] Transform playerTransform;
    [SerializeField] float damageMultiplier;
    [SerializeField] GameObject enemySpawnerPrefab;
    [SerializeField] GameObject dropItem;  // Item to drop when the boss dies
    [SerializeField] EnemyType enemyType;
    [SerializeField] Rigidbody rb;

    bool isShooting;
    bool isMelee;
    bool playerInRange;
    Color originalColor;
    Vector3 playerDirection;
    float angleToPlayer;

    int spawnThreshold1;
    int spawnThreshold2;
    bool spawnedSpawner = false;  // Ensure the spawner is spawned only once

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        originalColor = model.material.color;
        maxHp = hp;
        spawnThreshold1 = Mathf.RoundToInt(maxHp * 2 / 3f); // When HP drops below 2/3 (not used in current logic)
        spawnThreshold2 = Mathf.RoundToInt(maxHp * 1 / 3f); // When HP drops below 1/3
    }

    void Update()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        if (playerInRange || CanSeePlayer())
        {
            // Behavior when the player is visible is handled in CanSeePlayer()
        }
    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.isStopped = true;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
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
        GameObject obj = Instantiate(bullet, shootPos.position, transform.rotation);
        obj.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void FaceTarget()
    {
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    // Spawns the enemy spawner just under the boss on the ground.
    void SpawnEnemySpawner()
    {
        if (enemySpawnerPrefab != null)
        {
            Vector3 spawnPos;
            RaycastHit hit;
            // Cast a ray downward from the boss's position to find the ground
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
            {
                spawnPos = hit.point;
            }
            else
            {
                spawnPos = transform.position;
            }
            Instantiate(enemySpawnerPrefab, spawnPos, Quaternion.identity);
        }
    }

    // Drops an item when the boss dies.
    void DropItem()
    {
        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
       
        StartCoroutine(FlashRed());

        // Spawn the enemy spawner once when HP drops below 1/3.
        if (!spawnedSpawner && hp <= spawnThreshold2)
        {
            SpawnEnemySpawner();
            spawnedSpawner = true;
        }

        if (hp <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
    }
}
