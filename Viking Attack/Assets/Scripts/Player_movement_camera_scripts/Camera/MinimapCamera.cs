using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinimapCamera : NetworkBehaviour
{

    [SerializeField] private GameObject minimapCamera;
    void Awake()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");
    }

    public override void OnStartLocalPlayer()
    {
        if (minimapCamera != null)
        {
            minimapCamera.transform.SetParent(transform);
        }
    }
}
