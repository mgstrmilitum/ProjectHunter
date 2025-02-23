using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IDamageable, IPickable, TakeDamage
{
    [Header("-----Player Stats-----")]
    [SerializeField] public int currentHealth,maxHealth = 100;
    [SerializeField] public int currentShield,maxShield = 100;
    [SerializeField] public int currentAp, maxAp = 100;
    [SerializeField] public float combatTimer;
    [SerializeField] AudioSource combatMusic;
    [SerializeField] AudioSource nonCombatMusic;
    public float localCombatTimer;
    public bool abilityReady;
    public bool inCombat;
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
        if (localCombatTimer > 0 && inCombat == false)
        {
            inCombat = true;
            if (combatMusic.volume < 1)
            {
                combatMusic.volume += Time.deltaTime;
            }
            if (nonCombatMusic.volume > 0)
            {
                nonCombatMusic.volume -= Time.deltaTime;
            }
        }

        if (localCombatTimer <= 0 && inCombat == true)
        {
            inCombat = false;
            if (combatMusic.volume > 0)
            {
                combatMusic.volume -= Time.deltaTime;
            }
            if (nonCombatMusic.volume < 1)
            {
                nonCombatMusic.volume += Time.deltaTime;
            }
        }
        
        if (localCombatTimer >= 0)
        {
            localCombatTimer -= Time.deltaTime;
        }
    }
    void ShieldBehavior()
    {
        if (currentShield != 0) { shieldActive = true; }
        if (currentShield < 0) { currentShield = 0; }
        if (currentShield > maxShield) { currentShield = maxShield; }

    }

    public void TakeDamage(int amount)
    {
        localCombatTimer = combatTimer;
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
        localCombatTimer = combatTimer;
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
