using UnityEngine;
using UnityEngine.UI;


/**
 * @author Victor Wikner
 */
    public class UIPlayer : MonoBehaviour {
        //This script runs locally on every client, it decides what position in the player grid you are at in the lobby for players.
        [SerializeField] Text text;
        private Player _player;

        public void SetPlayer (Player _player) {
            this._player = _player;
            text.text = "Player " + _player.playerIndex.ToString ();
        }

    }

