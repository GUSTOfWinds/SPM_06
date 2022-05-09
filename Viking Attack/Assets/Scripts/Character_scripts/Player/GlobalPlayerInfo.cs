using Event;
using ItemNamespace;
using UnityEngine;


// WHO TO BLAME: Martin Kings

// Container for all player specifics, will add experience gained, HP, level, items owned etc...
public class GlobalPlayerInfo : MonoBehaviour
{
    [SerializeField] private Component healthBar;
    [SerializeField] private Component staminaBar;
    [SerializeField] private Component experienceBar;
    [SerializeField] private string playerName;
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


    private void Awake()
    {
        items = new ItemBase[4];
        damage = 5;
        health = 100;
        maxHealth = 100;
        stamina = 100;
        maxStamina = 100;
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
    }

    public void SetItemSlot(int index, ItemBase itemBase)
    {
        items[index] = itemBase;
    }

    // Gets called upon during game launch, the main menu sets the player name
    public void SetPlayerName(string insertedName)
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
        damage++;
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
}