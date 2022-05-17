using ItemNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


public class EnemyHealthBar : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] public Transform target; // the gameobject.transform that the UI should follow 
    [SerializeField] public Slider healthBar; // the slider 
    [SerializeField] private GameObject healthSource; // the enemy gameobject
    private EnemyVitalController enemyVitalController;
    private Vector3 wantedPos;

    [SerializeField]
    private uint netID; // the ID of the enemy spotted in the activation script placed on the player

    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        if (healthSource != null)
        {
            SetHealth();
            Display(); // Runs the display method that places the Ui element in the correct place above the enemy. Will only run if active.
        }
    }

    public void SetHealthSource(GameObject hs)
    {
        healthSource = hs;
        enemyVitalController = healthSource.GetComponent<EnemyVitalController>();
    }

    // Updates the health number of the slider
    public void SetHealth()
    {
        healthBar.value = enemyVitalController.GetCurrentHealth();
    }

    public uint GetPersonalNetID()
    {
        return netID;
    }

    public void Display()
    {
        wantedPos = mainCamera.WorldToScreenPoint(target.position);
        gameObject.transform.position = wantedPos;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Setup(Transform parent, Transform t, Transform enemy, EnemyInfo enemyInfo, Camera mainCam, uint netId)
    {
        transform.SetParent(parent.Find("UI"));
        target = t;
        SetHealthSource(enemy.gameObject);
        healthBar.maxValue = enemyInfo.maxHealth;
        this.netID = netId;
        mainCamera = mainCam;
    }
}