using UnityEngine;

namespace Main_menu_scripts.ForMP
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager;
        [Header("UI")] 
        [SerializeField] private GameObject landingPanel;


        public void HostLobby()
        {
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
    }
}
