using UnityEngine;

/**
 * @author Victor Wikner
 */
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager;
        [Header("UI")] 
        [SerializeField] private GameObject landingPanel;
        
        //MainMenu script only makes sure a server is started with the calling of StartHost()
        //once host has started, we remove the landingPanels visibility
        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }

