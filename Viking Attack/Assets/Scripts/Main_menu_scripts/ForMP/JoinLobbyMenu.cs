using TMPro;
using UnityEngine;
using UnityEngine.UI;
/**
 * @author Victor Wikner
 */
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;

        //OnEnable calls an event where a client gets connected or disconnected. OnDisable does the same except it throws the methods once a player leaves.
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

        //Joins a lobby based on the IP-input a player has made and then disables the joinbutton so they can't join an explicit amount of times
        public void JoinLobby()
        {
            string ipAddress = ipAddressInputField.text;
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = "localhost";
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
            joinButton.interactable = true;
        }
    
    
}
