using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] public int health;


    void Start()
    {
        
    }

    void Update()
    {

    }
    public void TakeDamage(int amount)
    {
        health -= amount;
    }
  
}
