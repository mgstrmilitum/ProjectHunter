using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface TakeDamage
{
    void takeDamage(int amount, Collider hitCollider);
}
