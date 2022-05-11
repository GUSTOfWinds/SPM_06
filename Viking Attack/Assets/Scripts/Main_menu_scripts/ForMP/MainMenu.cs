using UnityEngine;

namespace Main_menu_scripts.ForMP
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager;
        [Header("UI")] 
        [SerializeField] private GameObject landingPanel;

        [Header("Lobby")] 
        [SerializeField] private GameObject lobby;

        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
