using UnityEngine;
using UnityEngine.Rendering;

public class CrossBowBolt : MonoBehaviour
{
    
    private Player player;
    [SerializeField] public int damageAmount;
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((other.isTrigger))
        {
            return;
        }

        IDamageable dmg = other.GetComponent<IDamageable>();

        if (dmg != null)
        {
         
                dmg.TakeDamage(damageAmount);
            
       
        }
           Destroy(gameObject);
        
    }
}
