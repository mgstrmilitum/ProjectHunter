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
            
            desiredPos = enemyPos;
        }

       
        Instantiate(enemySpawnerPrefab, desiredPos, Quaternion.identity);
    }
}
