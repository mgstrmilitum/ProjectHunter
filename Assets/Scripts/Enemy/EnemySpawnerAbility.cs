using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnerAbility : MonoBehaviour
{
    [SerializeField] GameObject enemySpawnerPrefab; 
    [SerializeField] float spawnRadius = 2f;        

   
    public void SpawnEnemySpawner()
    {
        if (enemySpawnerPrefab == null)
        {
            Debug.LogWarning("Enemy spawner prefab is not assigned.");
            return;
        }

        
        Vector3 enemyPos = transform.position;

      
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 desiredPos = enemyPos + new Vector3(randomCircle.x, 0, randomCircle.y);

       
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(desiredPos, out navHit, spawnRadius, NavMesh.AllAreas))
        {
            desiredPos = navHit.position;
        }
        else
        {
            Debug.LogWarning("No valid NavMesh position found near enemy. Spawner will be instantiated at enemy's position.");
            desiredPos = enemyPos;
        }

       
        Instantiate(enemySpawnerPrefab, desiredPos, Quaternion.identity);
    }
}
