using System.Collections;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, TakeDamage
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
    float origStoppingDistance;
    public Transform weaponSlot;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
        origStoppingDistance = agent.stoppingDistance;
    }

    void Start()
    {
        GameManager.Instance.gameStats.enemiesTotal++;

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
        Debug.DrawRay(weaponSlot.position, -weaponSlot.right, Color.white);
        Debug.DrawRay(weaponSlot.position, -transform.right, Color.red);
        //angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        if ((playerInRange && !CanSeePlayer()))
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
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
        //animatorController.SetFloat("WalkSpeed", 0f);
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
        //Vector3 localLoweredHeadPos = headPos.position - new Vector3(0, 0.05f, 0);
        //Debug.DrawRay(localLoweredHeadPos, playerDirection.normalized * 20f, Color.green);
        
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.stoppingDistance = origStoppingDistance;
                agent.SetDestination(GameManager.Instance.player.transform.position);

                FaceTarget();
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    
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
            agent.stoppingDistance = origStoppingDistance;
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
        agent.isStopped = true;

        //turn to shoot
        FaceTarget();
        //TurnToShoot();

        animatorController.SetTrigger("Shoot");
        
        yield return new WaitForSeconds(shootRate);
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
        Vector3 lookDirection = new Vector3(playerDirection.x, 0f, playerDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        if (isShooting)
        {
            Vector3 eulerAngles = targetRotation.eulerAngles;
            eulerAngles.y += 91f; // guess and check this value
            targetRotation.eulerAngles = eulerAngles;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }

    void TurnToShoot()
    {
        transform.Rotate(transform.up, 90f);
    }

    void SpawnEnemySpawner()
    {
        //if (enemySpawnerPrefab != null && spawnerPositions.Length > 0)
        //{
        //    int randomIndex = Random.Range(0, spawnerPositions.Length);
        //    Instantiate(enemySpawnerPrefab, spawnerPositions[randomIndex].position, Quaternion.identity);
        //}
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        GameManager.Instance.gameStats.shotsHit++;
        agent.SetDestination(GameManager.Instance.player.transform.position);
        animatorController.SetFloat("WalkSpeed", 0.5f);
        if (co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        StartCoroutine(FlashRed());
        if (hp <= 0)
        {
            //GameManager.Instance.gameStats.enemiesRemaining--;
            GameManager.Instance.gameStats.numKills++;
            Destroy(gameObject);
        }
    }

    void ShootProjectile()
    {
        GameObject obj = Instantiate(bullet, weaponSlot.position, Quaternion.identity);

        obj.GetComponent<Rigidbody>().AddForce(-transform.right * 20f, ForceMode.Impulse);
    }
}
