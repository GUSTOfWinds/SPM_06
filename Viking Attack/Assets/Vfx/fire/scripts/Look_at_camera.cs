using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.fire
{
    public class Look_at_camera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector3 v3 = new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
            this.transform.LookAt(v3);
        }
    }
}
