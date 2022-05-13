using UnityEngine;
using UnityEngine.UI;

namespace Main_menu_scripts.ForMP
{
    public class UIPlayer : MonoBehaviour {

        [SerializeField] Text text;
        Player player;

        public void SetPlayer (Player player) {
            this.player = player;
            text.text = "Player " + player.playerIndex.ToString ();
        }

    }
}
