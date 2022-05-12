using UnityEngine;
using Mirror;

public class MinimapCamera : NetworkBehaviour
{
    [SerializeField] private GameObject minimapCamera;

    void Awake()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");
    }

    public override void OnStartAuthority()
    {
        if (minimapCamera != null)
        {
            minimapCamera.transform.SetParent(transform);
            minimapCamera.transform.localPosition = new Vector3(0, 200, 0);
        }
    }
}