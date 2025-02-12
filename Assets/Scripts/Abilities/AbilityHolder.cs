using UnityEngine;

public class AbilityHolder : MonoBehaviour
{

    public Ability ability;

    
    private void Update()
    {
        if (Input.GetButtonDown("Ability"))
        {
            ability.Activate(gameObject);
        }
    }
}







