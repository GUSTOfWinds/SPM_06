using UnityEngine;

namespace ItemNamespace
{
    // Creates the ScriptableObject function for the Item objects.
    [CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Create new character")]

    // Contains the base information for all characters.
    public class CharacterBase : ScriptableObject
    {
        /**
         * @author Martin Kings
         */
        [SerializeField] private EnemyType type;

        [SerializeField] private string soName;
        [SerializeField] private string description;
        [SerializeField] private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float damage;
        [SerializeField] private float chasingSpeedMultiplier;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float maxHealth;
        [SerializeField] private float experienceRadius;
        [SerializeField] private float experience;
        [SerializeField] private int staggerStamina; // example: Sword does 2 stagger stamina enemy has 3, it takes 2 hit for sword to stagger

        public int GetStaggerStamina()
        {
            return staggerStamina;
        }
        // Returns name of the item
        public string GetScriptableObjectName()
        {
            return soName;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        // Returns the description of the item
        public string GetDescription()
        {
            return description;
        }

        // Returns the range of the character
        public float GetRange()
        {
            return range;
        }

        // Returns the attack cooldown of the character
        public float GetAttackCooldown()
        {
            return attackCooldown;
        }


        // Returns the base damage of the character
        public float GetDamage()
        {
            return damage;
        }

        public float GetChasingSpeed()
        {
            return chasingSpeedMultiplier;
        }

        public float GetMovementSpeed()
        {
            return moveSpeed;
        }

        public float GetExperienceRadius()
        {
            return experienceRadius;
        }

        // Will give more exp if higher level
        public float GetExperience()
        {
            return experience;
        }

        // Contains the different item type, add a new line to the enum in order to add an item type.
        public enum EnemyType
        {
            Skeleton,
            Bear
        }

        public EnemyType GetEnemyType()
        {
            return type;
        }
    }
}
