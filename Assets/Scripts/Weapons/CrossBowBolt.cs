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

        TakeDamage dmg = other.GetComponent<TakeDamage>();

        if (dmg != null)
        {
         
                dmg.takeDamage(damageAmount);
            
       
        }
           Destroy(gameObject);
        
    }
}
