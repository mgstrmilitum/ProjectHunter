using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GuardBoss : MonoBehaviour, TakeDamage
{
    [Header("Common Stats")]
    [SerializeField] int hp;
    [SerializeField] int maxHp;

    [Header("Ranged (Phase 1) Settings")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootFOV;

    [Header("Melee (Phase 2) Settings")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRate;       // Total time for one melee cycle
    [SerializeField] float roarDuration;    // Duration of the "Roar" animation
    [SerializeField] float phase2SpeedMultiplier = 1.5f;

    [Header("Animation")]
    [SerializeField] Animator animatorController; // Expects triggers "Roar" & "Swing" and a bool "Walk"

    [Header("References")]
    [SerializeField] Renderer model;
    [SerializeField] Renderer[] models;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform headPos;
    [SerializeField] Transform playerTransform; // (Optional, if needed)

    // Internal state variables.
    Vector3 startingPos;
    bool isShooting;
    bool isAttacking;
    bool hasRoared;
    bool isWalking;
    bool playerInRange;
    bool phase2Activated = false; // Ensures phase 2 initialization runs once.

    int berserkThreshold; // When hp <= this, switch to phase 2.

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        phase2Activated = false;
        startingPos = transform.position;
        maxHp = hp;
        berserkThreshold = Mathf.RoundToInt(maxHp * 0.5f);
    }

    void Update()
    {
        if (hp > berserkThreshold)
        {
            // Ensure the boss remains at its starting position.
            if (Vector3.Distance(transform.position, startingPos) > 0.1f)
            {
                agent.SetDestination(startingPos);
            }

            // Only process shooting if the player is inside the trigger (playerInRange is true).
            if (playerInRange)
            {
                // Calculate an adjusted target position using the player's x and z but the boss's shootPos.y.
                Vector3 playerPos = GameManager.Instance.player.transform.position;
                Vector3 adjustedPlayerPos = new Vector3(playerPos.x, playerPos.y, playerPos.z);
                Vector3 direction = (adjustedPlayerPos - shootPos.position).normalized;

                // Only shoot if the player is within the shooting FOV.
                float angleToPlayer = Vector3.Angle(transform.forward, direction);
                if (angleToPlayer <= shootFOV)
                {
                    FaceTargetRanged(direction);
                    if (!isShooting)
                    {
                        StartCoroutine(ShootRanged(direction));
                    }
                }
            }
        }
        else
        {
            
            
            if (!phase2Activated)
            {
                agent.speed *= phase2SpeedMultiplier;
                
                StartCoroutine(PlayRoarThenWalk());
                

                phase2Activated = true;
            }

            // Always chase the player.
            isWalking = true;
            animatorController.SetFloat("WalkSpeed", 1f);
            agent.SetDestination(GameManager.Instance.player.transform.position);
            
            
            // When within stopping distance, stop walking and perform the melee attack.
            if (agent.remainingDistance <= agent.stoppingDistance)
            {

                if (isWalking)
                {
                    animatorController.SetFloat("WalkSpeed", 0f);
                    isWalking = false;
                }
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                // If the player moves away, ensure the boss is playing its walking animation.
                if (!isWalking)
                {
                    animatorController.SetFloat("WalkSpeed", 1f);
                    isWalking = true;
                }
            }
        }
    }

    #region Ranged Methods (Phase 1)
    IEnumerator ShootRanged(Vector3 direction)
    {
        isShooting = true;
        animatorController.SetTrigger("Shoot");
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        GameObject obj = Instantiate(bullet, shootPos.position, bulletRotation);
        obj.GetComponent<Rigidbody>().AddForce(direction * 20f, ForceMode.Impulse);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void FaceTargetRanged(Vector3 targetDirection)
    {
        Vector3 lookDirection = new Vector3(targetDirection.x, 0f, targetDirection.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }
    #endregion

    #region Melee Methods (Phase 2)
    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        // Stop moving while attacking.
        agent.isStopped = true;
        // Trigger the swing animation.
        animatorController.SetBool("Swing",true);
        Debug.Log("Swing animation triggered");

        // Wait for half of the melee cycle (simulate wind-up).
        yield return new WaitForSeconds(meleeRate * 0.5f);

        // Deal damage to the player.
        TakeDamage damageable = GameManager.Instance.player.GetComponentInParent<TakeDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(meleeDamage);
        }

        // Wait for the remainder of the melee cycle.
        yield return new WaitForSeconds(meleeRate * 0.5f);
        animatorController.SetBool("Swing", true);
        agent.isStopped = false;
        isAttacking = false;
    }

    // Plays the "Roar" animation then switches to "Walk."
    IEnumerator PlayRoarThenWalk()
    {
        animatorController.SetTrigger("Roar");
        yield return new WaitForSeconds(roarDuration);
    }

    
    #endregion

    #region Utility Methods
    IEnumerator FlashRed()
    {
        foreach (Renderer rend in models)
        {
            rend.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer rend in models)
        {
            rend.material.color = model.material.color;
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        // On hit, update the destination to the player's current position.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(GameManager.Instance.player.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        StartCoroutine(FlashRed());
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Trigger Handling
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
        }
    }
    #endregion
}
