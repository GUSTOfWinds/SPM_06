using Mirror;
using UnityEngine;


    public class AutoHostClient : MonoBehaviour {

        
        //TODO no use as of 2022-05-16
        //this script checks if there is a host locally, then it proceeds to join if it is.
        [SerializeField] private NetworkManager networkManager;

        void Start () {
            if (!Application.isBatchMode) { //Headless build
                Debug.Log ($"=== Client Build ===");
                networkManager.StartClient ();
            } else {
                Debug.Log ($"=== Server Build ===");

            }
        }

        //JoinLocal is run from a button-press and if someone has the same IP as you you automatically join that IP.
        public void JoinLocal () {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient ();
        }

    }

