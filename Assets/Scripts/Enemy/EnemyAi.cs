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

    [SerializeField] int hp;
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

    float angleToPlayer;
    float stoppingDistanceOrig;
    bool isShooting;
    bool isMelee;
    bool playerInRange;
    bool isRoaming;
    Color originalColor;
    Vector3 playerDirection;
    Vector3 startingPos;
    Vector3 loweredHeadPos;

    Coroutine co;

    [SerializeField]
    EnemyType enemyType;

    void Start()
    {
        originalColor = model.material.color;
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    void Update()
    {
        Vector3 loweredHeadPos = headPos.position - new Vector3(0, 0.2f, 0);
        playerDirection = playerTransform.position - loweredHeadPos;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        if (playerInRange)
        {
            agent.SetDestination(playerTransform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (CanSeePlayer() && !isShooting && enemyType != EnemyType.Melee && angleToPlayer <= shootFOV)
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

    bool CanSeePlayer()
    {
       

        // Fix Debug Ray Stretching and Lower Start Position
        Debug.DrawRay(loweredHeadPos, playerDirection.normalized * 20f, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(loweredHeadPos, playerDirection.normalized, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.SetDestination(playerTransform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                if (!isShooting && enemyType != EnemyType.Melee && angleToPlayer <= shootFOV)
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
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject obj = Instantiate(bullet, shootPos.position, transform.rotation);
        if (enemyType == EnemyType.Grenade)
        {
            obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * grenadeSpeed, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(playerTransform.position);
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
