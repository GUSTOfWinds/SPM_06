using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakableInfo : NetworkBehaviour
{
    private Transform respawnParent;

    public void SetRespawnAnchor(Transform p)
    {
        respawnParent = p;
    }

    public Transform GetRespawnParent()
    {
        return respawnParent;
    }
}
