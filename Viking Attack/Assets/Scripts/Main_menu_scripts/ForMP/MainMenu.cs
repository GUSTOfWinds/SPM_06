using UnityEngine;


namespace Main_menu_scripts.ForMP
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel;

        public void HostLobby()
        {
            networkManager.StartHost();

            landingPagePanel.SetActive(false);
        }
    }
}