using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;
using UnityEngine.Networking.Types;
using Inventory_scripts;


public class GlobalPlayerInfo : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Component healthBar;

    [SerializeField] private Component staminaBar;
    [SerializeField] private Component experienceBar;
    [SyncVar] [SerializeField] private string playerName;
    [SyncVar] [SerializeField] private Color skinColor;
    [SyncVar] [SerializeField] private float health;
    [SyncVar] [SerializeField] private float maxHealth;
    [SerializeField] private ItemBase[] items;
    [SyncVar] [SerializeField] private bool alive;
    [SyncVar] [SerializeField] private float stamina;
    [SyncVar] [SerializeField] private float maxStamina;
    [SyncVar] [SerializeField] private float experience;
    [SyncVar] [SerializeField] private int level;
    [SyncVar] [SerializeField] private float levelThreshold;
    [SyncVar] [SerializeField] private int availableStatpoints;
    [SyncVar] [SerializeField] private float damage;
    [SyncVar] [SerializeField] private int meatStackNumber;
    [SyncVar] [SerializeField] private int armorLevel;
    //get character Screen
    [SerializeField] private GameObject charatS;
    


    //gathering data and reset data
    public void LoadData(Dictionary<String, System.Object> data)
    {
       
        playerName = (string)data["playerName"];
        health = (float)data["health"];
        stamina = (float)data["stamina"];
        experience = (float)data["experience"];
        level = (int)data["level"];
        availableStatpoints = (int)data["availableStatpoints"];
        //damageStat = (int)dataDict["damageStat"];
        //healthStat = (int)dataDict["healthStat"];
        //staminaStat = (int)dataDict["staminaStat"];
        meatStackNumber = (int)data["meatStackNumber"];
        damage = (float)data["damage"];
        armorLevel = (int)data["armorLevel"];
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
        experienceBar.GetComponent<PlayerExperienceBar>().SetExperience(experience);
        charatS.GetComponent<CharacterScreen>().OpenCharacterScreen();
        gameObject.GetComponent<PlayerInventory>().UpdateMeatStack();
    }
    //Saving all the data
    public Dictionary<String, System.Object> SaveData()
    {

        Dictionary<String, System.Object> dataHolder = new Dictionary<string, System.Object>();
       // Dictionary<String, Dictionary<String, System.Object>> dataToSave = new Dictionary<string, Dictionary<String, System.Object>>();
        dataHolder.Add("playerName", (System.Object)playerName);
        dataHolder.Add("health", (System.Object)health);
        dataHolder.Add("stamina", (System.Object)stamina);
        dataHolder.Add("experience", (System.Object)experience);
        dataHolder.Add("level", (System.Object)level);
        dataHolder.Add("availableStatpoints", (System.Object)availableStatpoints);
        //dataHolder.Add("damageStat", (System.Object)damageStat);
        //dataHolder.Add("healthStat", (System.Object)healthStat);
        //dataHolder.Add("staminaStat", (System.Object)staminaStat);
        dataHolder.Add("meatStackNumber", (System.Object)meatStackNumber);
        dataHolder.Add("damage", (System.Object)damage);
        dataHolder.Add("armorLevel", (System.Object)armorLevel);
        return dataHolder;
    }






    //*******************


    private void Awake()
    {
        alive = true;
        items = new ItemBase[5];
        damage = 100;
        health = 100;
        maxHealth = 100;
        stamina = 100;
        maxStamina = 120;
        healthBar = gameObject.transform.Find("UI").gameObject.transform.Find("Health_bar").gameObject.transform
            .Find("Health_bar_slider").gameObject.GetComponent<PlayerHealthBar>();
        staminaBar = gameObject.transform.Find("UI").gameObject.transform.Find("Stamina_bar").gameObject.transform
            .Find("Stamina_bar_slider").gameObject.GetComponent<PlayerStaminaBar>();
        experienceBar = gameObject.transform.Find("UI").gameObject.transform.Find("Experience_bar").gameObject.transform
            .Find("Experience_bar_slider").gameObject.GetComponent<PlayerExperienceBar>();
        experience = 0;
        levelThreshold = 60;
        availableStatpoints = 0;
        level = 1;
        playerName = PlayerPrefs.GetString("PlayerName");
        armorLevel = 0;
    }


    public void IncreaseArmorLevel(int increase)
    {
        armorLevel += increase;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            CmdSetPlayerName(PlayerPrefs.GetString("PlayerName"));
        }
    }

    public void SetItemSlot(int index, ItemBase itemBase)
    {
        items[index] = itemBase;
    }

    // Gets called upon during game launch, the main menu sets the player name
    [Command]
    public void CmdSetPlayerName(string insertedName)
    {
        playerName = insertedName;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Gets called upon during game launch, the main menu sets the player skin color
    public void SetSkinColor(Color insertedColor)
    {
        skinColor = insertedColor;
    }

    // Returns the player name
    public string GetName()
    {
        return playerName;
    }

    // Returns the player skin color
    public Color GetSkinColor()
    {
        return skinColor;
    }

    // Checks if the player is alive
    public bool IsAlive()
    {
        return alive;
    }

    // Adds or reduces health
    public void UpdateHealth(float difference)
    {
        if (health + difference <= maxHealth)
        {
            if (health + difference < 0)
            {
                health = 0;
            }
            else
            {
                health += difference;
            }
        }
        else if (health + difference > maxHealth)
        {
            health = maxHealth;
        }

        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        if (health <= 0)
        {
            // Used by PlayerActivateEnemyHealthBar class on player objects and by 
            // RespawnPanelHandler
            EventInfo playerDeathEvent = new PlayerDeathEventInfo
            {
                EventUnitGo = gameObject,
                playerNetId = gameObject.GetComponent<NetworkIdentity>().netId
            };
            EventSystem.Current.FireEvent(playerDeathEvent);
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    }

    public void SetHealth(float hp)
    {
        health = hp;
    }

    // Returns the current stamina
    public float GetStamina()
    {
        return stamina;
    }

    // Returns the players max stamina
    public float GetMaxStamina()
    {
        return maxStamina;
    }

    // Adds or reduces stamina
    public void UpdateStamina(float difference)
    {
        if (stamina + difference <= maxStamina)
        {
            stamina += difference;
        }
        else
        {
            stamina = maxStamina;
        }

        staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
    }


    // Increases the players current experience, will reset it to 0 if the player reaches next level
    public void IncreaseExperience(float exp)
    {
        experience += exp;
        if (experience >= levelThreshold * (1.3 * level))
        {
            IncreaseLevel();
            experience = 0;
        }

        experienceBar.GetComponent<PlayerExperienceBar>().SetExperience(experience);
    }

    public void IncreaseLevel()
    {
        EventInfo playerLevelUpInfo = new PlayerLevelUpEventInfo
        {
            netID = gameObject.GetComponent<NetworkIdentity>().netId
        };
        EventSystem.Current.FireEvent(playerLevelUpInfo);
        level++;
        availableStatpoints += 3;
    }

    public float GetExperience()
    {
        return experience;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetStatPoints()
    {
        return availableStatpoints;
    }

    public void IncreaseDamageStatPoints()
    {
        damage += 8;
        availableStatpoints--;
    }

    public void IncreaseHealthStatPoints()
    {
        maxHealth += 15;
        health += 15;
        availableStatpoints--;
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
    }

    public void IncreaseStaminaStatPoints()
    {
        maxStamina += 10;
        availableStatpoints--;
        staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
    }

    public float GetLevelThreshold()
    {
        return levelThreshold;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void IncreaseMeatStackNumber()
    {
        meatStackNumber++;
    }

    public int GetMeatStackNumber()
    {
        return meatStackNumber;
    }

    public void DecreaseMeatStackNumber()
    {
        meatStackNumber--;
    }

    public void SetDisplayName(string playerName)
    {
        this.playerName = playerName;
    }

    public int GetArmorLevel()
    {
        return armorLevel;
    }
}