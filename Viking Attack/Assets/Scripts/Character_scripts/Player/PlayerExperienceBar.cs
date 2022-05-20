using UnityEngine;
using UnityEngine.UI;


    public class PlayerExperienceBar : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        public Slider experienceBar; // the slider 

        [SerializeField] private GlobalPlayerInfo globalPlayerInfo; // contains the global info of the current player
        public Text levelText;

        private void Start()
        {
            experienceBar.maxValue = (globalPlayerInfo.GetLevel() * globalPlayerInfo.GetLevelThreshold() * 1.3f);
            experienceBar.value = globalPlayerInfo.GetExperience();
            levelText.text = globalPlayerInfo.GetLevel().ToString();
        }

        // Updates the value of the slider to the players current stamina (will be called upon when attackacking, sprinting etc)
        public void SetExperience(float experience)
        {
            experienceBar.maxValue = (globalPlayerInfo.GetLevel() * (globalPlayerInfo.GetLevelThreshold() * 1.3f));
            experienceBar.value = experience;
            levelText.text = globalPlayerInfo.GetLevel().ToString();
        }
    }
