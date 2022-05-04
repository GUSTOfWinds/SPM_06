using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu_scripts
{
    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TMP_InputField nameInputField;

        [SerializeField] private Button continueButton;

        public static string DisplayName
        {
            get;
            private set;
        }

        private const string PlayerPrefsNameKey = "PlayerName";

        private void Start() => SetUpInputField();

        private void SetUpInputField()
        {
            if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;

            var defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;
            SetPlayerName(defaultName);
        }

        public void SetPlayerName(string userName)
        {
            continueButton.interactable = !string.IsNullOrEmpty(userName);
        }

        public void SavePlayerName()
        {
            DisplayName = nameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
        }
    }
}
