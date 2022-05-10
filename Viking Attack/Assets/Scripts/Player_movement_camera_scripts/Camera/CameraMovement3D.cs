using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;
using Camera = UnityEngine.Camera;

public class CameraMovement3D : NetworkBehaviour
{
    [SerializeField] private GameObject firstPersonPosition;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] private bool lockMouse;
    private float rotationX;
    private float rotationY;
    private Vector3 cameraPosition;
    private Camera mainCamera;
    public bool shouldBeLocked; // used by the character screen to lock the mouse movement when character screen is open
    

    void Awake()
    {
        shouldBeLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        
    }

    public override void OnStartLocalPlayer()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.position = firstPersonPosition.transform.position;
        }

        if (isLocalPlayer)
        {
            gameObject.transform.Find("UI").GetComponent<Canvas>().enabled = true;
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
        mainCamera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
    public void LockCamera ()
    {

        rotationX = 0;
        rotationY = 0;
        rotationX = Mathf.Clamp(rotationX, -89, 89);
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}