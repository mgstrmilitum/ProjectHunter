using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporttrigger : MonoBehaviour
{
    public Teleporter teleporter;

    private void OnTriggerEnter(Collider collider)
    {
        var teleportable = collider.GetComponent<AbletoTeleport>();
        if (teleportable != null)
        {
            teleporter.OnEnter(teleportable);
        }
    }
}


