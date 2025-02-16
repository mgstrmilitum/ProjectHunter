using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Vector3 teleportLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // teleports the player to the teleportation location when they enter the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = teleportLocation;
        }
    }
}
