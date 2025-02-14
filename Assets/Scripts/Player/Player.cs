using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IDamageable, IPickable
{
    [Header("-----Player Stats-----")]
    [SerializeField] public int currentHealth,maxHealth = 100;
    [SerializeField] public int currentShield,maxShield = 100;
    bool shieldActive;



    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        shieldActive = true;
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TakeDamage(10);
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



}
