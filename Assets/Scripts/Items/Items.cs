using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;

public class Items : MonoBehaviour, IPickable
{
    [SerializeField] ItemIDS ID;
    public Material material;
    enum ItemIDS
    {
        Health,
        Shield,
    }


    public void OnPickup(Collider other)
    {
        //checking if the object that entered is the player
        if (other.isTrigger) { return; }
        Player player = other.transform.GetComponentInParent<Player>();

        if (player != null)
        {
            switch (ID)
            {
                case ItemIDS.Health:

                    player.GainHealth(50);
                    Destroy(gameObject);
                    break;

                case ItemIDS.Shield:

                    player.GainShield(100);
                    Destroy(gameObject);
                    break;

            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //if the object is a child of IPickable then execute OnPickup()

     IPickable player = other.GetComponent<IPickable>();

        if (player != null)
        {
            OnPickup(other);
        }
    }

}
