using UnityEngine;
using Mirror;

public class FogScript : NetworkBehaviour
{

    [SerializeField] private GameObject fog;
    void Awake()
    {
        fog = GameObject.FindGameObjectWithTag("Fog");
    }

    public override void OnStartLocalPlayer()
    {
        if (fog != null)
        {
            fog.transform.SetParent(transform);
            fog.transform.localPosition = new Vector3 (0f, 0f, 200f);
            fog.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }
}
