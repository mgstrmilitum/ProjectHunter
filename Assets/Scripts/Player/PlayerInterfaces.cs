using UnityEngine;

public class PlayerInterfaces : MonoBehaviour, IOpen, IpickupWeapons
{
    private bool keyInHand;
    public bool hasKey()
    {
        return keyInHand;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Key")) keyInHand = true;
    }
}
