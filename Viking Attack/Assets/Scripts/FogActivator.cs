using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogActivator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject fog;
    [SerializeField] private GameObject destroyObject;

    private void Awake()
    {
        fog.SetActive(true);
    }

    private void Start()
    {
        GameObject.Destroy(destroyObject);
    }
}
