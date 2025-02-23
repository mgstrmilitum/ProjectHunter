using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class VampireBossAi : MonoBehaviour, TakeDamage
{
    [Header("Common Stats")]
    [SerializeField] int hp;
    [SerializeField] int maxHp;

    [Header("Ranged Settings")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;    // Time between shots
    [SerializeField] int shootFOV;
    [SerializeField] int shootDamage;    // Base shoot damage

    [Header("Spawner Settings")]
    [SerializeField] GameObject spawnerPrefabPhase1; // For Phase 1
    [SerializeField] GameObject spawnerPrefabPhase5; // For Phase 5
    [SerializeField] Transform[] spawnerPositions;     // Designated positions for spawners

    [Header("Animation")]
    [SerializeField] Animator animatorController; // Expects triggers "Shoot", "Swing", "Roar"

    [Header("References")]
    [SerializeField] Renderer model;
    [SerializeField] Renderer[] models;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform playerTransform; // (Optional – if needed)

    // Internal state
    Vector3 startingPos;
    bool isShooting = false;
    bool playerInRange = false;

    // Phase thresholds (hp values)
    int threshold75;
    int threshold50;
    int threshold25;

    // Flags so each phase transition happens only once.
    bool phase1Started = false;   // Triggered when player is first in range.
    bool phase2Triggered = false; // 75% threshold
    bool phase3Triggered = false; // 50% threshold
    bool phase4Triggered = false; // 25% threshold
    bool revived = false;         // Determines if Phase 5 (revival) has been used

    // For phase 3 and 4 looping effects
    Coroutine swingCoroutine = null;
    Coroutine flashingYellowCoroutine = null;

    // For invincibility during phase 4 flashing
    bool invincible = false;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        startingPos = transform.position;
        maxHp = hp;
        // Calculate thresholds
        threshold75 = Mathf.RoundToInt(maxHp * 0.75f);
        threshold50 = Mathf.RoundToInt(maxHp * 0.50f);
        threshold25 = Mathf.RoundToInt(maxHp * 0.25f);
    }

    void Update()
    {
        // Always try to remain near starting position.
        if (Vector3.Distance(transform.position, startingPos) > 0.1f)
        {
            agent.SetDestination(startingPos);
        }

        // In Phase 1 (ranged behavior), if the player is in range, face and shoot.
        if (playerInRange)
        {
            // Phase 1 spawner summoning occurs only once when the player is first detected.
            if (!phase1Started)
            {
                phase1Started = true;
                SummonSpawnersPhase1();
            }

            Vector3 playerPos = GameManager.Instance.player.transform.position;
            Vector3 direction = (playerPos - shootPos.position).normalized;
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

    #region Ranged Methods
    IEnumerator ShootRanged(Vector3 direction)
    {
        isShooting = true;
        animatorController.SetTrigger("Shoot");
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        GameObject obj = Instantiate(bullet, shootPos.position, bulletRotation);

        // Fire the bullet. The bullet prefab already handles damage.
        obj.GetComponent<Rigidbody>().AddForce(direction * 20f, ForceMode.Impulse);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void FaceTargetRanged(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, faceTargetSpeed * Time.deltaTime);
    }
    #endregion

    #region Phase Methods

    // Phase 2: Heal all enemies (only once) by directly adding 25 to their hp.
    void HealEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // This line finds the Enemy component attached to the enemy GameObject.
            var enemyScript = enemy.GetComponent<MeleeEnemyAi>();
            if (enemyScript != null)
            {
                enemyScript.hp += 25;
            }
        }
        Debug.Log("Phase 2 triggered: Healed all enemies by 25 hp.");
    }

    // Phase 3: Start the swing attack loop.
    IEnumerator SwingAttackLoop()
    {
        while (true)
        {
            animatorController.SetTrigger("Swing");
            // This line freezes the player's movement for 1.5 seconds.
            GameManager.Instance.player.SendMessage("FreezeMovement", 1.5f, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(5f);
        }
    }

    // Phase 4: Flash yellow and become invincible.
    IEnumerator FlashYellowLoop()
    {
        while (true)
        {
            invincible = true;
            foreach (Renderer rend in models)
            {
                rend.material.color = Color.yellow;
            }
            yield return new WaitForSeconds(3f); // 3 seconds flashing (invincible)
            foreach (Renderer rend in models)
            {
                rend.material.color = model.material.color;
            }
            invincible = false;
            yield return new WaitForSeconds(3f); // Wait for remaining cycle (total 6 seconds)
        }
    }

    // Phase 5: Revive the boss and spawn Phase 5 spawners.
    void Phase5Revive()
    {
        if (revived) return;

        revived = true;
        // Spawn Phase 5 spawners at the same positions as Phase 1.
        SummonSpawnersPhase5();

        // Stop Phase 3 and 4 effects if running.
        if (swingCoroutine != null)
        {
            StopCoroutine(swingCoroutine);
            swingCoroutine = null;
        }
        if (flashingYellowCoroutine != null)
        {
            StopCoroutine(flashingYellowCoroutine);
            flashingYellowCoroutine = null;
        }
        agent.isStopped = true;
        StartCoroutine(Phase5Routine());
    }

    IEnumerator Phase5Routine()
    {
        // Pause and flash black for 3 seconds while roaring.
        foreach (Renderer rend in models)
        {
            rend.material.color = Color.black;
        }
        animatorController.SetTrigger("Roar");
        yield return new WaitForSeconds(3f);
        foreach (Renderer rend in models)
        {
            rend.material.color = model.material.color;
        }
        // Remove bullet damage increase; only speed up the shoot rate.
        shootRate /= 1.5f;
        // Restore hp to the 50% threshold.
        hp = threshold50;
        agent.isStopped = false;
        Debug.Log("Phase 5 triggered: Boss revived with improved shoot rate and Phase 5 spawners summoned.");
    }

    #endregion

    #region Damage and Threshold Checks
    public void takeDamage(int amount)
    {
        if (invincible)
        {
            // Ignore damage while invincible.
            return;
        }

        hp -= amount;

        // Update destination to the player's current position.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(GameManager.Instance.player.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        StartCoroutine(FlashRed());

        // Check phase thresholds (each only once).
        if (!phase2Triggered && hp <= threshold75)
        {
            phase2Triggered = true;
            HealEnemies();
        }
        if (!phase3Triggered && hp <= threshold50)
        {
            phase3Triggered = true;
            swingCoroutine = StartCoroutine(SwingAttackLoop());
            Debug.Log("Phase 3 triggered: Swing attack loop started.");
        }
        if (!phase4Triggered && hp <= threshold25)
        {
            phase4Triggered = true;
            flashingYellowCoroutine = StartCoroutine(FlashYellowLoop());
            Debug.Log("Phase 4 triggered: Flashing yellow invincibility started.");
        }

        // Phase 5: If hp reaches 0 or below.
        if (hp <= 0)
        {
            if (!revived)
            {
                Phase5Revive();
            }
            else
            {
                OnDeath();
                Destroy(gameObject);
            }
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
            rend.material.color = model.material.color;
        }
    }

    void OnDeath()
    {
        Debug.Log("Boss has been defeated.");
        // Add any death effects (explosions, score updates, etc.) here.
    }
    #endregion

    #region Utility Methods
    void SummonSpawnersPhase1()
    {
        if (spawnerPrefabPhase1 != null && spawnerPositions != null && spawnerPositions.Length > 0)
        {
            foreach (Transform spawnPoint in spawnerPositions)
            {
                Instantiate(spawnerPrefabPhase1, spawnPoint.position, spawnPoint.rotation);
            }
            Debug.Log("Phase 1: Spawners summoned at designated positions.");
        }
        else
        {
            Debug.LogWarning("Phase 1 Spawner Prefab or Spawner Positions not assigned.");
        }
    }

    void SummonSpawnersPhase5()
    {
        if (spawnerPrefabPhase5 != null && spawnerPositions != null && spawnerPositions.Length > 0)
        {
            foreach (Transform spawnPoint in spawnerPositions)
            {
                Instantiate(spawnerPrefabPhase5, spawnPoint.position, spawnPoint.rotation);
            }
            Debug.Log("Phase 5: Spawners summoned at designated positions.");
        }
        else
        {
            Debug.LogWarning("Phase 5 Spawner Prefab or Spawner Positions not assigned.");
        }
    }
    #endregion

    #region Trigger Handling
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!phase1Started)
            {
                phase1Started = true;
                SummonSpawnersPhase1();
            }
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
