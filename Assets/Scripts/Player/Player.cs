using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IDamageable, IPickable, TakeDamage
{
    [Header("-----Player Stats-----")]
    [SerializeField] public int currentHealth,maxHealth = 100;
    [SerializeField] public int currentShield,maxShield = 100;
    [SerializeField] public int currentAp, maxAp = 100;
    public bool abilityReady;
    bool shieldActive;



    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentAp = 0;
        shieldActive = true;
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
    }

    public void GainHealth(int amountToGain) { currentHealth += amountToGain; if (currentHealth > maxHealth) { currentHealth = maxHealth; } }

    public void GainShield(int amountToGain){currentShield += amountToGain;}

    public void GainAp(int amountToGain) { currentAp += amountToGain;}

}
