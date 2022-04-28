using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;
using Camera = UnityEngine.Camera;

public class CameraMovement3D : NetworkBehaviour
{
    [SerializeField]private GameObject firstPersonPosition;
    [SerializeField]private GameObject thirdPersonPosition;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] private bool lockMouse;
    private float rotationX;
    private float rotationY;
    private Vector3 cameraPosition;
    private Camera mainCamera;

    private void Update()
    {
        // Just for the testing, allows the player to control if the mouse should be locked
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnStartLocalPlayer()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            if (GetComponentInParent<PlayerScript3D>().firstPerson)
            {
                cameraPosition = firstPersonPosition.transform.localPosition;
            }
            else
            {
                cameraPosition = thirdPersonPosition.transform.localPosition;

            }
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

    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        //Looks if camera hits a collider and if it's true change position of camera to hit position
        Vector3 cameraOffset = mainCamera.transform.rotation * cameraPosition;
        RaycastHit hit;
        if (Physics.SphereCast(mainCamera.transform.parent.transform.position, 0.1f, cameraOffset, out hit, cameraOffset.magnitude, ~(1 << mainCamera.transform.parent.gameObject.layer)))
        {
            mainCamera.transform.position = mainCamera.transform.parent.transform.position + cameraOffset.normalized * hit.distance;
        }
        else
        {
            mainCamera.transform.position = mainCamera.transform.parent.transform.position + cameraOffset;
        }
    }
    public void OnMouseMovement(InputAction.CallbackContext value)
    {
        if (!isLocalPlayer) return;
        //Sets rotation to camera depending on mouse position and movement
        rotationX -= value.ReadValue<Vector2>().y * mouseSensitivity;
        rotationY += value.ReadValue<Vector2>().x * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -89, 89);
        mainCamera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
