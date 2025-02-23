using UnityEngine;

public class AbilityHolder : MonoBehaviour
{

    public Ability ability;

    
    private void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            ability.Activate(gameObject);
        }
    }
}







