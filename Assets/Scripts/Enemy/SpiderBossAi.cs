
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
    int spawnThreshold2;

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
        spawnThreshold1 = Mathf.RoundToInt(maxHp * 2 / 3f); // When HP drops below 2/3
        spawnThreshold2 = Mathf.RoundToInt(maxHp * 1 / 3f); // When HP drops below 1/3
    }

    void Update()
    {
        // Calculate a lowered head position and update player direction and angle.
        //Vector3 loweredHeadPos = headPos.position - new Vector3(0, 0.2f, 0);
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        //angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        if ((playerInRange && !CanSeePlayer()))
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                //co = StartCoroutine(Roam());
            }

        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                //co = StartCoroutine(Roam());
            }
        }
    }

    //IEnumerator Roam()
    //{
    //    isRoaming = true;
    //    yield return new WaitForSeconds(roamPauseTime);
    //    agent.stoppingDistance = 0;
    //    Vector3 randomPos = Random.insideUnitSphere * roamDistance;
    //    randomPos += startingPos;
    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);
    //    agent.SetDestination(hit.position);
    //    isRoaming = false;
    //}

    // Only checks if the enemy can see the player.
    bool CanSeePlayer()
    {
        //Vector3 localLoweredHeadPos = headPos.position - new Vector3(0, 0.05f, 0);
        //Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * 20f, Color.green);

        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.SetDestination(GameManager.Instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                if (!isShooting && angleToPlayer <= shootFOV)
                {
                    StartCoroutine(Shoot());

                }

                agent.stoppingDistance = stoppingDistanceOrig;
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

    void SpawnEnemySpawner()
    {
        if (enemySpawnerPrefab != null && spawnerPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnerPositions.Length);
            Instantiate(enemySpawnerPrefab, spawnerPositions[randomIndex].position, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;

        agent.SetDestination(GameManager.Instance.player.transform.position);
        if (co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        StartCoroutine(FlashRed());
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}


