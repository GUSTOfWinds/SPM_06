
using ItemNamespace;
using UnityEngine;


// WHO TO BLAME: Martin Kings

// Container for all player specifics, will add experience gained, HP, level, items owned etc...
public class GlobalPlayerInfo : MonoBehaviour
{
    public string playerName;
    public Color skinColor;
    public float health;
    public float maxHealth;
    public ItemBase[] items;
    public bool alive = true;
    public float stamina;
    public float maxStamina;
    public Component healthBar;
    public Component staminaBar;
    public Component experienceBar;
    public float experience;
    public int level;
    public float levelThreshold;
    public int availableStatpoints;
    private int damageStat;
    private int healthStat;
    private int staminaStat;

    private void Awake()
    {
        health = 100;
        maxHealth = 100;
        stamina = 100;
        maxStamina = 100;
        healthBar = gameObject.transform.Find("UI").gameObject.transform.Find("Health_bar").gameObject.transform.Find("Health_bar_slider").gameObject.GetComponent<PlayerHealthBar>();
        staminaBar = gameObject.transform.Find("UI").gameObject.transform.Find("Stamina_bar").gameObject.transform.Find("Stamina_bar_slider").gameObject.GetComponent<PlayerStaminaBar>();
        experienceBar = gameObject.transform.Find("UI").gameObject.transform.Find("Experience_bar").gameObject.transform.Find("Experience_bar_slider").gameObject.GetComponent<PlayerExperienceBar>();
        experience = 0;
        levelThreshold = 30;
        availableStatpoints = 0;
        level = 1;
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
        if (health <= 0) {
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
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
        if (experience >= levelThreshold * (1.3 * level)) {
            IncreaseLevel();
            experience = 0;
        }

        experienceBar.GetComponent<PlayerExperienceBar>().SetExperience(experience);
    }

    public void IncreaseLevel()
    {
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
        // TODO add increased base damage to player
        availableStatpoints--;
        damageStat++;
    }
    
    public void IncreaseHealthStatPoints()
    {
        // TODO add increased base health to player
        availableStatpoints--;
        healthStat++;
    }

    public void IncreaseStaminaStatPoints()
    {
        // TODO add increased base stamina to player
        availableStatpoints--;
        staminaStat++;
    }

    

}