using Mirror;
using UnityEngine;

/**
 * @author Victor Wikner
 */
public class NetworkGamePlayer : NetworkBehaviour
{

    [SyncVar]
    public string displayName = "Loading...";

    [SyncVar] 
    public Color32 colour;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
        
    //Sets name on your character in lobby and latter the game, OnStartAuthority makes sure it is run only on the object that is yours.
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.displayName);
        CmdSetPlayerColour(PlayerNameInput.playerColour);
    }
        //Syncs displayname between all players
    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
        //syncs players colour between all players
    [Command]
    private void CmdSetPlayerColour(Color32 colour32)
    {
        colour = colour32;
    }

    //when client is started on this object we make sure the data isn't destroyed upon moving to the next scene
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        //Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    public void SetSkinColour(Color32 colour)
    {
        this.colour = colour;
    }
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}