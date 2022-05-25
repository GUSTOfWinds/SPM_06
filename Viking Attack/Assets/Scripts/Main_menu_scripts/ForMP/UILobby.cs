using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Victor Wikner
 * Not implemented, would function differently than current
 */

    public class UILobby : MonoBehaviour {

        public static UILobby instance;

        [Header ("Host Join")]
        [SerializeField] TMP_InputField joinMatchInput;
        [SerializeField] List<Selectable> lobbySelectables = new List<Selectable> ();
        [SerializeField] Canvas lobbyCanvas;
        [SerializeField] Canvas searchCanvas;
        bool searching = false;

        [Header ("Lobby")]
        [SerializeField] Transform UIPlayerParent;
        [SerializeField] GameObject UIPlayerPrefab;
        [SerializeField] TMP_Text matchIDText;
        [SerializeField] GameObject beginGameButton;
        [SerializeField] TMP_Text readyText;


        GameObject localPlayerLobbyUI;
        
        //Sets current instance to the same as this lobby
        void Start () {
            instance = this;
        }

        //Sets start button active for the owner of the lobby
        public void SetStartButtonActive (bool active) {
            beginGameButton.SetActive (active);
        }

        //Choose if you want a searchable game or not
        public void HostPublic () {
            lobbySelectables.ForEach (x => x.interactable = false);
            Debug.Log("Player: " + Player.localPlayer.ToString());
            
            Player.localPlayer.HostGame (true);
        }

        public void HostPrivate () {
            lobbySelectables.ForEach (x => x.interactable = false);

            Player.localPlayer.HostGame (false);
        }

        //Starts hosting and adds you to the match, if you already has a lobby ui it will be destroyed
        public void HostSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (Player.localPlayer);
                matchIDText.text = matchID;
            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }

        //joins lobby
        public void Join () {
            lobbySelectables.ForEach (x => x.interactable = false);

            Player.localPlayer.JoinGame (joinMatchInput.text.ToUpper ());
        }

        //Adds you to the lobby if you have
        public void JoinSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (Player.localPlayer);
                matchIDText.text = matchID;
            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }
        
        //Leaves the game and removes lobby from your canvas

        public void DisconnectGame () {
            if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
            Player.localPlayer.DisconnectGame ();

            lobbyCanvas.enabled = false;
            lobbySelectables.ForEach (x => x.interactable = true);
        }

        //Adds your prefab to the lobby
        public GameObject SpawnPlayerUIPrefab (Player player) {
            GameObject newUIPlayer = Instantiate (UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer> ().SetPlayer (player);
            newUIPlayer.transform.SetSiblingIndex (player.playerIndex - 1);

            return newUIPlayer;
        }

        public void BeginGame () {
            Player.localPlayer.BeginGame ();
        }

        public void SearchGame () {
            StartCoroutine (Searching ());
        }

        public void CancelSearchGame () {
            searching = false;
        }

        public void SearchGameSuccess (bool success, string matchID) {
            if (success) {
                searchCanvas.enabled = false;
                searching = false;
                JoinSuccess (success, matchID);
            }
        }

        IEnumerator Searching () {
            searchCanvas.enabled = true;
            searching = true;

            float searchInterval = 1;
            float currentTime = 1;

            while (searching) {
                if (currentTime > 0) {
                    currentTime -= Time.deltaTime;
                } else {
                    currentTime = searchInterval;
                    Player.localPlayer.SearchGame ();
                }
                yield return null;
            }
            searchCanvas.enabled = false;
        }

    }
