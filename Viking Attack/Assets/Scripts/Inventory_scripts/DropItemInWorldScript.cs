using UnityEngine;
using ItemNamespace;

namespace Inventory_scripts
{
    public class DropItemInWorldScript : MonoBehaviour
    {
        /*
            @Author Love Strignert - lost9373
        */
        public ItemBase itembase;
        [SerializeField] private float rotationSpeed = 1;
        private ParticleSystem particleSystem;
        private GameObject itemRender;
        private float rotation;

        void Start()
        {
            itemRender = transform.Find("ItemRender").gameObject;
            particleSystem = itemRender.GetComponent<ParticleSystem>();
            if(itembase != null)
            {
                //Gets the mesh and material from the itembase that is droped
                itemRender.GetComponent<MeshFilter>().mesh = itembase.GetMesh;
                itemRender.GetComponent<MeshRenderer>().material = itembase.GetMaterial;
                var shape = particleSystem.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Mesh;
                shape.mesh = itembase.GetMesh;
            }
        }

        public void SetUp(ItemBase itemBase)
        {
            itembase = itemBase;
            itemRender.GetComponent<MeshFilter>().mesh = itembase.GetMesh;
            itemRender.GetComponent<MeshRenderer>().material = itembase.GetMaterial;
            var shape = particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Mesh;
            shape.mesh = itembase.GetMesh;
        }

        void Update()
        {
            //Gives a roatating animation to the droped item
            if(rotation > 0)
            {
               rotation += rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0,rotation,0); 
            }
        }
    }
}
