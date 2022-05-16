using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu_scripts.ForMP
{
    public class NetworkRoomPlayerLobby : NetworkBehaviour
    {

        [Header("UI")] 
        [SerializeField] private GameObject lobbyUI = null;

        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[5];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[5];
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private Button readyButton = null;


        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady = false;

        [SyncVar(hook = nameof(HandleColourChanged))]
        public Color colour;

        private bool isLeader;
        public bool IsLeader
        {
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);
            }
        }

        private NetworkManagerLobby room;

        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }

        }

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.displayName);
            lobbyUI.SetActive(true);
        }

        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);
            UpdateDisplay();
        }
    
        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);
            UpdateDisplay();
        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

        public void HandleColourChanged(Color oldValue, Color newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {

            if (!hasAuthority)
            {

                
                foreach (var roomplayer in Room.RoomPlayers)
                {
                    if(!isLocalPlayer) lobbyUI.SetActive(false);

                }
                foreach (var player in Room.RoomPlayers)
                {
                    if (player.hasAuthority)
                    {

                        player.UpdateDisplay();
                        break;
                    }
                }
                return;
            }

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                playerReadyTexts[i].text = string.Empty;
            }

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
                playerNameTexts[i].color = NameColour();
                playerReadyTexts[i].text = Room.RoomPlayers[i].isReady
                    ? "<color=#9CFF8D> Ready </color>"
                    : "<color=#E53737> Not Ready </color>";
            }
        }

        private Color32 NameColour()
        {
            Color32 returnColour;
            if (!isLocalPlayer || !hasAuthority) return returnColour = new Color32(255,255,255,255);
            byte red = (byte)PlayerPrefs.GetInt("redValue");
            byte green = (byte)PlayerPrefs.GetInt("greenValue");
            byte blue = (byte)PlayerPrefs.GetInt("blueValue");
            returnColour = new Color32(r: red, g: green, b: blue, a: 255);

            return returnColour;
        }

        public void HandleReadyToStart(bool readyToStart)
        {

            if (!isLeader) return;
            startGameButton.interactable = readyToStart;
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {

            isReady = !isReady;
            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;
            Room.StartGame();    
        }
    }
}