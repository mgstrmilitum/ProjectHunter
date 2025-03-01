using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IpickupWeapons, TakeDamage, IOpen
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
    bool shieldActive;
    private bool inCombat;
    public bool key;
    public PlayerStats stats;

    [Header("Collectibles")]
    public int numGarlic;

    public struct PlayerStats
    {
        public int shotsFired;
        public int shotsHit;
        public int enemiesKilled;
    }

    void Start()
    {
        inCombat = false;
        currentHealth = GameManager.Instance.statsSO.currentHealth;
        currentShield = GameManager.Instance.statsSO.currentShield;
        currentAp = GameManager.Instance.statsSO.currentSpecial;
        shieldActive = true;
        numGarlic = GameManager.Instance.statsSO.currentGarlic;
        GameManager.Instance.SetHealthWithoutLerp();
    }

   
    void Update()
    {
        if (localCombatTimer > 0 && inCombat == false)
        {
            inCombat = true;
            if (combatMusic.volume < 1)
            {
                combatMusic.volume += 25f * Time.deltaTime;
            }
            if (nonCombatMusic.volume > 0)
            {
                nonCombatMusic.volume -= 25f * Time.deltaTime;
            }
        }

        if (localCombatTimer <= 0 && inCombat == true)
        {
            inCombat = false;
            if (combatMusic.volume > 0)
            {
                combatMusic.volume -= 10f * Time.deltaTime;
            }
            if (nonCombatMusic.volume < 1)
            {
                nonCombatMusic.volume += 10f * Time.deltaTime;
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
        if (currentHealth <= 0) {
            GameManager.Instance.OnLose();
        }

    }

    public void GainHealth(int amountToGain) { currentHealth += amountToGain; if (currentHealth > maxHealth) { currentHealth = maxHealth; } }

    public void GainShield(int amountToGain){currentShield += amountToGain;}

    public void GainAp(int amountToGain) { currentAp += amountToGain;}

    public void ResetAp() { currentAp = 0;}

    public bool hasKey()
    {
        return key;
    }

    public void AddGarlic()
    {
        numGarlic++;
        GameManager.Instance.garlicCount.text = numGarlic.ToString();
        StartCoroutine(GameManager.Instance.ShowGarlicStats());
    }
}
