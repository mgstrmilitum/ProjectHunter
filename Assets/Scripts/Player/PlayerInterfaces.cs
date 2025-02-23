using UnityEngine;

public class PlayerInterfaces : MonoBehaviour, IOpen, IPickable, TakeDamage
{
    private Player player;
    private bool keyInHand;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    public bool hasKey()
    {
        return keyInHand;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Key")) keyInHand = true;
    }

    public void takeDamage(int amount)
    {
        player.takeDamage(amount);
    }
}
