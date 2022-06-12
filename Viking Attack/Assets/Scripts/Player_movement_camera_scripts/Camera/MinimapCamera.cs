using Mirror;
using System.Collections;
using UnityEngine;


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
                minimapCamera.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        }
        }

}
