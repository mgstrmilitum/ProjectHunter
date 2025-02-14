using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public int currentHealth;
    public int maxHealth;
    public int currentShield;
    public int maxShield;
    bool shieldActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // records player stats
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            currentHealth = other.GetComponent<Player>().currentHealth;
            maxHealth = other.GetComponent<Player>().maxHealth;
            currentShield = other.GetComponent<Player>().currentShield;
            maxShield = other.GetComponent<Player>().maxShield;
        }
    }
}
