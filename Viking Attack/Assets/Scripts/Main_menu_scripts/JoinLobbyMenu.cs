using Main_menu_scripts.ForMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Main_menu_scripts
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager;

        [Header("UI")] 
        [SerializeField] private GameObject landingPagePanel;

        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private Button joinButton;

        private void OnEnable()
        {
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby()
        {
            string ipAddress = ipAddressInputField.text;
            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }
        private void HandleClientDisconnected()
        {
            joinButton.interactable = false;

        }

    }
}