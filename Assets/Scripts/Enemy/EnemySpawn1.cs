using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn1 : MonoBehaviour, TakeDamage
{
    [SerializeField] GameObject[] enemyPrefabs; // Array of enemy prefabs to spawn.
    [SerializeField] int numToSpawn = 3;          // Number of enemies to spawn at once.
    [SerializeField] float spawnRadius = 3f;        // Radius around the spawner for enemy spawn positions.
    [SerializeField] int hp = 50;                   // Health of the spawner.
    [SerializeField] float spawnDelay = 5f;         // Delay before automatically spawning enemies.

    bool hasSpawnedEnemies = false; // Ensures enemies are spawned only once.

    private void Start()
    {
        // Begin the delayed spawn process.
        StartCoroutine(DelayedSpawnEnemies());
    }

    IEnumerator DelayedSpawnEnemies()
    {
        yield return new WaitForSeconds(spawnDelay);
        // Only spawn if the spawner hasn't been destroyed.
        if (!hasSpawnedEnemies)
        {
            SpawnEnemies();
            hasSpawnedEnemies = true;
            Destroy(gameObject);
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            // If the spawner is destroyed by damage, do not spawn any enemies.
            Destroy(gameObject);
        }
    }

    // Spawns the enemies near the spawner's position on the NavMesh.
    void SpawnEnemies()
    {
        Vector3 spawnerPosition = transform.position;
        for (int i = 0; i < numToSpawn; i++)
        {
            // Generate a random offset within a circle around the spawner.
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 desiredPosition = spawnerPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Ensure the spawn position is on the NavMesh.
            NavMeshHit hit;
            if (NavMesh.SamplePosition(desiredPosition, out hit, spawnRadius, NavMesh.AllAreas))
            {
                desiredPosition = hit.position;
            }

            // Select a random enemy prefab and instantiate it at the validated position.
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], desiredPosition, Quaternion.identity);
        }
    }
}
