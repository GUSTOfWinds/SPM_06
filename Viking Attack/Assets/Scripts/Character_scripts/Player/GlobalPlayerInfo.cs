using System;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;
using Inventory_scripts;


public class GlobalPlayerInfo : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Component healthBar;

    [SyncVar] [SerializeField] public Color32 skinColour;
    [SerializeField] public SkinnedMeshRenderer skinMesh;
    private int red, green, blue;
    [SerializeField] private Component staminaBar;
    [SerializeField] private Component experienceBar;
    [SyncVar] [SerializeField] private string playerName;
    [SyncVar] [SerializeField] private float health;
    [SyncVar] [SerializeField] private float maxHealth;
    [SerializeField] private ItemBase[] items;
    [SyncVar] [SerializeField] private bool alive;
    [SyncVar] [SerializeField] private float stamina;
    [SyncVar] [SerializeField] private float maxStamina;
    [SyncVar] [SerializeField] private float experience;
    [SyncVar] [SerializeField] private int level;
    [SyncVar] [SerializeField] private float levelThreshold;
    [SyncVar] [SerializeField] private int availableStatPoints;
    [SyncVar] [SerializeField] private float damage;
    [SyncVar] [SerializeField] private int meatStackNumber;

    [SyncVar] [SerializeField] private int armorLevel;

    //get character Screen
    [SerializeField] private GameObject characterScreen;


    //gathering data and reset data
    public void LoadData(Dictionary<String, System.Object> data)
    {
        playerName = (string) data["playerName"];
        health = (float) data["health"];
        stamina = (float) data["stamina"];
        experience = (float) data["experience"];
        level = (int) data["level"];
        availableStatPoints = (int) data["availableStatPoints"];
        //damageStat = (int)dataDict["damageStat"];
        //healthStat = (int)dataDict["healthStat"];
        //staminaStat = (int)dataDict["staminaStat"];
        meatStackNumber = (int) data["meatStackNumber"];
        damage = (float) data["damage"];
        armorLevel = (int) data["armorLevel"];
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
        experienceBar.GetComponent<PlayerExperienceBar>().SetExperience(experience);
        characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
        gameObject.GetComponent<PlayerInventory>().UpdateMeatStack();
    }

    //Saving all the data
    public Dictionary<String, System.Object> SaveData()
    {
        Dictionary<String, System.Object> dataHolder = new Dictionary<string, System.Object>();
        // Dictionary<String, Dictionary<String, System.Object>> dataToSave = new Dictionary<string, Dictionary<String, System.Object>>();
        dataHolder.Add("playerName", (System.Object) playerName);
        dataHolder.Add("health", (System.Object) health);
        dataHolder.Add("stamina", (System.Object) stamina);
        dataHolder.Add("experience", (System.Object) experience);
        dataHolder.Add("level", (System.Object) level);
        dataHolder.Add("availableStatPoints", (System.Object) availableStatPoints);
        //dataHolder.Add("damageStat", (System.Object)damageStat);
        //dataHolder.Add("healthStat", (System.Object)healthStat);
        //dataHolder.Add("staminaStat", (System.Object)staminaStat);
        dataHolder.Add("meatStackNumber", (System.Object) meatStackNumber);
        dataHolder.Add("damage", (System.Object) damage);
        dataHolder.Add("armorLevel", (System.Object) armorLevel);
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
        availableStatPoints = 0;
        level = 1;
        playerName = PlayerPrefs.GetString("PlayerName");
        armorLevel = 0;
        red = PlayerPrefs.GetInt("redValue"); // Victor Wikner
        green = PlayerPrefs.GetInt("greenValue"); // Victor Wikner
        blue = PlayerPrefs.GetInt("blueValue");// Victor Wikner
        skinColour = new Color32((byte) red, (byte) green, (byte) blue, 255); // Victor Wikner

        skinMesh.material.SetColor(BaseColor, skinColour); //Victor Wikner
    }


    public void IncreaseArmorLevel(int increase)
    {
        armorLevel += increase;
    }

    /**
    * @author Victor Wikner
    */
    private NetworkManagerLobby room;
    private static readonly int BaseColor = Shader.PropertyToID("Color_be8b5dda336745c985841ed4b814c54e");

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            if (isServer)
            {
                UpdateColours();
            }

            CmdSetPlayerName(playerName);
            CmdSetSkinColour(skinColour);
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
    /**
    * @author Victor Wikner
    */
    // Gets called upon during game launch, the main menu sets the player name
    [Command]
    public void CmdSetSkinColour(Color32 chosenColour)
    {
        this.skinColour = chosenColour;
        UpdateColours();
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /**
    * @author Victor Wikner
    */
    private void UpdateColours()
    {
        foreach (var t in room.InGamePlayer)
        {
            t.GetComponent<GlobalPlayerInfo>().skinMesh.material
                .SetColor(BaseColor, t.GetComponent<GlobalPlayerInfo>().skinColour);
        }
    }

    
    // Returns the player name
    public string GetName()
    {
        return playerName;
    }

    /**
 * @author Victor Wikner
 */
    // Returns the player skin color
    public Color32 GetSkinColor()
    {
        return skinColour;
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
                EventUnitGo = gameObject
            };
            EventSystem.Current.FireEvent(playerDeathEvent);
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    }

    /**
    * @author Victor Wikner
     * Not Implemented
    */
    /*private void UpdateDisplay()
    {


        //This loop sets the name of a current player in the lobby and sets the name of the colour they've chosen.
        //Todo remove NameColour call when we have character customization available
        for (int i = 0; i < Room.InGamePlayer.Count; i++)
        {
            Color32 currentColour = room.InGamePlayer.
            room.InGamePlayer[i]..material.SetColor(BaseColor, color32);

            playerNameTexts[i].color = Room.RoomPlayers[i].colour;

        }
    }*/
    
    /**
 * @author Victor Wikner
 */
    public void SetSkinColour(Color32 insertedColor)
    {
        skinColour = insertedColor;
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
        availableStatPoints += 3;
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
        return availableStatPoints;
    }

    public void IncreaseDamageStatPoints()
    {
        damage += 6;
        availableStatPoints--;
    }

    public void IncreaseHealthStatPoints()
    {
        maxHealth += 10;
        availableStatPoints--;
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
    }

    public void IncreaseStaminaStatPoints()
    {
        maxStamina += 10;
        availableStatPoints--;
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

    /**
    * @author Victor Wikner
    */
    [Server]
    public void SetDisplayName(string playersName)
    {
        this.playerName = playersName;
    }

    public int GetArmorLevel()
    {
        return armorLevel;
    }
    /**
    * @author Victor Wikner
    */
    public override void OnStartClient()
    {
        if (Room != null) Room.InGamePlayer.Add(this.gameObject);
    }
    /**
    * @author Victor Wikner
    */
    public override void OnStopClient()
    {
        if (Room != null) Room.InGamePlayer.Remove(this.gameObject);
    }
}