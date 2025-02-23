using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IDamageable, IPickable, TakeDamage, IOpen
{
    [Header("-----Player Stats-----")]
    
    [SerializeField] public int currentHealth,maxHealth = 100;
    [SerializeField] public int currentShield,maxShield = 100;
    [SerializeField] public int currentAp, maxAp = 100;
    public bool abilityReady;
    bool shieldActive;
    public bool key;
    public PlayerStats stats;

    [Header("Collectibles")]
    public int numGarlic;

    public struct PlayerStats
    {
        int shotsFired;
        int shotsHit;
        int enemiesKilled;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentAp = 0;
        shieldActive = true;
        numGarlic = 0;
    }

   
    void Update()
    {
       
    }
    void ShieldBehavior()
    {
        if (currentShield != 0) { shieldActive = true; }
        if (currentShield < 0) { currentShield = 0; }
        if (currentShield > maxShield) { currentShield = maxShield; }

    }

    public void TakeDamage(int amount)
    {
        ShieldBehavior();
        if (shieldActive)
        {
            currentShield -= amount;
            ShieldBehavior();
            if (currentShield <= 0)
            {
                shieldActive = false;
            }
            return;
        }
        currentHealth -= amount;
    }

    public void takeDamage(int amount)
    {
        if (shieldActive)
        {
            currentShield -= amount;
            ShieldBehavior();
            if (currentShield <= 0)
            {
                shieldActive = false;
            }
            return;
        }
        currentHealth -= amount;
        if (currentHealth <= 0) {
            GameManager.Instance.OnLose();
        }

    }

    public void GainHealth(int amountToGain) { currentHealth += amountToGain; if (currentHealth > maxHealth) { currentHealth = maxHealth; } }

    public void GainShield(int amountToGain){currentShield += amountToGain;}

    public void GainAp(int amountToGain) { currentAp += amountToGain;}

    public void ResetAp() { currentAp = 0; }
    public bool hasKey()
    {
        return key;
    }

    public void AddGarlic()
    {
        numGarlic++;
        Debug.Log("Garlic count: " + numGarlic);
        GameManager.Instance.garlicCount.text = numGarlic.ToString();
        StartCoroutine(GameManager.Instance.ShowGarlicStats());
    }
}
