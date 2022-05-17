using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;



public class GlobalPlayerInfo : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Component healthBar;
    [SerializeField] private Component staminaBar;
    [SerializeField] private Component experienceBar;
    [SyncVar] [SerializeField] private string playerName;
    [SerializeField] private Color skinColor;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private ItemBase[] items;
    [SerializeField] private bool alive = true;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float experience;
    [SerializeField] private int level;
    [SerializeField] private float levelThreshold;
    [SerializeField] private int availableStatpoints;
    [SerializeField] private int damageStat;
    [SerializeField] private int healthStat;
    [SerializeField] private int staminaStat;
    [SerializeField] private float damage;
    [SerializeField] private int meatStackNumber;
    [SerializeField] private int armorLevel;


    private void Awake()
    {
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

    public void IncreaseArmorLevel()
    {
        armorLevel++;
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
            health += difference;
        }
        else
        {
            health = maxHealth;
        }

        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        if (health <= 0)
        {
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

    public int GetDamageStatPoints()
    {
        return damageStat;
    }

    public int GetHealthStatPoints()
    {
        return healthStat;
    }

    public int GetStaminaStatPoints()
    {
        return staminaStat;
    }

    public void IncreaseDamageStatPoints()
    {
        damage+=6;
        availableStatpoints--;
        damageStat++;
    }

    public void IncreaseHealthStatPoints()
    {
        maxHealth += 10;
        availableStatpoints--;
        healthStat++;
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
    }

    public void IncreaseStaminaStatPoints()
    {
        maxStamina += 10;
        availableStatpoints--;
        staminaStat++;
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