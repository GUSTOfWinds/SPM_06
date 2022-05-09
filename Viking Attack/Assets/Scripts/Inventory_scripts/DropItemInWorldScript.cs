using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemNamespace;

public class DropItemInWorldScript : MonoBehaviour
{
    public ItemBase itembase;
    [SerializeField] private float rotationSpeed = 1;
    private float rotation;

    void Start()
    {
        if(itembase != null)
        {
            //Gets the mesh and material from the itembase that is droped
            gameObject.GetComponent<MeshFilter>().mesh = itembase.GetMesh;
            gameObject.GetComponent<MeshRenderer>().material = itembase.GetMaterial;
        }
    }
    void Update()
    {
        //Gives a roatating animation to the droped item
        rotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0,rotation,0);
    }
}
