using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public KeyCode SwordKey = KeyCode.X;
    public Ability ability;

    
    private void Update()
    {
        if (Input.GetKey(SwordKey))
        {
            ability.Activate(gameObject);
        }
    }
}







