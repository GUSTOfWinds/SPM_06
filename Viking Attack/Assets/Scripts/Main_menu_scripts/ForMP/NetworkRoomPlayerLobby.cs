using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu_scripts.ForMP
{
    public class NetworkRoomPlayerLobby : NetworkBehaviour
    {
        //In short this script is the player whilst in the lobby, it provides the name, and the the colour of the player, 
        [Header("UI")] 
        [SerializeField] private GameObject lobbyUI = null;

        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[5];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[5];
        [SerializeField] private TMP_Text readyButtonText = null;
        [SerializeField] private TMP_Text playButtonText = null;
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private Button readyButton = null;


        //These are syncvariables that updates towards the server, and when there is a change we run the methods called in the hook
        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string displayName = "Loading...";

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady = false;

        [SyncVar(hook = nameof(HandleColourChanged))]
        public Color colour;

        //isLeader variable and IsLeader method runs to set who can start the game and who can't.
        private bool isLeader;
        public bool IsLeader
        {
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);
            }
        }
        
        //room and Room are supposed to function with the server, and if there isn't a room that holds the NetworkManager it'll create one
        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }

        }

        //Sets name on your character in lobby and latter the game, OnStartAuthority makes sure it is run only on the object that is yours.
        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.displayName);
            lobbyUI.SetActive(true);
        }

        //Method runs when you join a server, it adds your own UI, and then sends a command to the server to update all the users with the information.
        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);
            UpdateDisplay();
        }
    
        //This is the reverse of OnStartClient() and is thrown whenever a client disconnects, and it removes the current player from the lobby.
        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);
            UpdateDisplay();
        }
        
        //Following 3 methods updates what is shown whenever a value is changed for the client.
        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        public void HandleColourChanged(Color oldValue, Color newValue) => UpdateDisplay();

        //Method makes sure all players display the same thing whilst in the lobby. It checks to see if it's not the local player and disables the ui element so that there is only one shown per client.
        private void UpdateDisplay()
        {

            if (!hasAuthority)
            {
                //First foreach decides if the lobby UI is active or not base on if you're the local player or not.
                foreach (var roomPlayer in Room.RoomPlayers)
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
            //Sets the name as waiting for player before anyone has joined the lobby, and makes sure there is no ready-status when there is no player.
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                playerReadyTexts[i].text = string.Empty;
            }
            //This loop sets the name of a current player in the lobby and sets the name of the colour they've chosen.
            //Todo remove NameColour call when we have character customization available
            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].displayName;
                playerNameTexts[i].color = NameColour();
                playerReadyTexts[i].text = Room.RoomPlayers[i].isReady
                    ? "<color=#9CFF8D> Ready </color>"
                    : "<color=#8C3333> Not Ready </color>";
                
            }
        }

        //Sets the colour of the players name based on the RGB scale on the badge which represents the player.
        //Color32 is used over Color as Color didn't show 
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
    
        //if you aren't the leader you can't start the game. readyToStart works by comparing every players ready status and changes the intractability of the start button based on that.
        //All players in lobby must be ready
        public void HandleReadyToStart(bool readyToStart)
        {
            if(readyToStart)
                readyButtonText.text = "<color=#9CFF8D>Ready </color>";
            if(!readyToStart)
                readyButtonText.text = "<color=white>Ready </color>";
            if (!isLeader) return;
            startGameButton.interactable = readyToStart;
            if (readyToStart)
                playButtonText.text = "<color=white>start game </color>";
            if (!readyToStart)
                playButtonText.text = "<color=#A6A6A6>start game </color>";
        }

        //Following 2 command methods gets the name and ready status on the server and sets on clients.
        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {

            isReady = !isReady;
            Room.NotifyPlayersOfReadyState();
        }

        //Only host can start game, and throws the command all over the server
        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;
            Room.StartGame();    
        }
    }
}