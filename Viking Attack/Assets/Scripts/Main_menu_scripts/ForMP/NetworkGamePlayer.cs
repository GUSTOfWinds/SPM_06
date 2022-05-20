using Mirror;
using UnityEngine;


    public class NetworkGamePlayer : NetworkBehaviour
    {

        [SyncVar]
        public string displayName = "Loading...";

        [SyncVar] 
        public Color32 colour = new Color32(200, 50, 200, 255);

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            Room.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetSkinColour(Color32 colour)
        {
            this.colour = colour;
        }
        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
    }
