using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player_movement_camera_scripts.Camera
{
    public class CameraMovement3D : NetworkBehaviour
    {
        [SerializeField] private GameObject firstPersonPosition;
        [SerializeField] float mouseSensitivity = 1;
        [SerializeField] private bool lockMouse;
        [SerializeField] private Canvas canvas;
        private float rotationX;
        private float rotationY;
        private Vector3 cameraPosition;
        private UnityEngine.Camera mainCamera;
        public bool shouldBeLocked; // used by the character screen to lock the mouse movement when character screen is open
    


        void Awake()
        {
            shouldBeLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<UnityEngine.Camera>();
        }

        public override void OnStartLocalPlayer()
        {
            if (mainCamera != null)
            {
                Transform transform1;
                (transform1 = mainCamera.transform).SetParent(transform);
                transform1.position = firstPersonPosition.transform.position;
                transform1.rotation = firstPersonPosition.transform.rotation;
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                canvas.enabled = false;
            }
        }

        public override void OnStopLocalPlayer()
        {
            if (mainCamera != null)
            {
                mainCamera.transform.SetParent(null);
                SceneManager.MoveGameObjectToScene(mainCamera.gameObject, SceneManager.GetActiveScene());
                mainCamera.orthographic = true;
                mainCamera.transform.localPosition = new Vector3(0f, 70f, 0f);
                mainCamera.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        public void OnMouseMovement(InputAction.CallbackContext value)
        {
            if (!isLocalPlayer || !shouldBeLocked) return;
            //Sets rotation to camera depending on mouse position and movement
            rotationX -= value.ReadValue<Vector2>().y * mouseSensitivity;
            rotationY += value.ReadValue<Vector2>().x * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, -89, 89);
            mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }
}