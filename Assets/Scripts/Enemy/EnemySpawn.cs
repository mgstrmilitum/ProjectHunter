using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject[] spawnObjects;
    [SerializeField] public int numToSpawn;
    [SerializeField] float spawnTime;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] public List<GameObject> spawnList;

    public int spawnCount;

    bool isSpawning;
    bool startSpawning;
    public bool isObjectiveSpawner;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && spawnCount < numToSpawn && !isSpawning)
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        int transformArrayPosition = Random.Range(0, spawnPos.Length);
        int objectArrayPosition = Random.Range(0, spawnObjects.Length);
        GameObject instantiated = Instantiate(spawnObjects[objectArrayPosition], spawnPos[transformArrayPosition].position, spawnPos[transformArrayPosition].rotation);
        spawnList.Add(instantiated);
        instantiated.transform.SetParent(transform);
        spawnCount++;
        isSpawning = false;
        yield return new WaitForSeconds(spawnTime);


    }
}
