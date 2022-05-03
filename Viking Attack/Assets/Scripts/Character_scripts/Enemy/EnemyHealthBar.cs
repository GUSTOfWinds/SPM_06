using System;
using ItemNamespace;
using UnityEngine;
using UnityEngine.UI;

// WHO TO BLAME: Martin Kings

// This script will make a GUITexture follow a transform (object placed above the enemy).
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] public Transform target; // the gameobject.transform that the UI should follow 
    [SerializeField] public Slider healthBar; // the slider 
    [SerializeField] private GameObject healthSource; // the enemy gameobject

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
    }

    // Updates the health number of the slider
    public void SetHealth()
    {
        healthBar.value = healthSource.GetComponent<EnemyVitalController>().getCurrentHealth();
    }

    public uint GetPersonalNetID()
    {
        return netID;
    }

    public void Display()
    {
        var wantedPos = mainCamera.WorldToScreenPoint(target.position);
        gameObject.transform.position = wantedPos;
    }

    public void Setup(Transform parent, Transform t, Transform enemy, EnemyInfo enemyInfo, Camera mainCam, uint netID)
    {
        transform.SetParent(parent.Find("UI"));
        target = t;
        SetHealthSource(enemy.gameObject);
        healthBar.maxValue = enemyInfo.maxHealth;
        this.netID = netID;
        mainCamera = mainCam;
    }
}