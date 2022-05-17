using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;

namespace Main_menu_scripts.ForMP.Försök2 {

    [System.Serializable]
    public class Match {
        public string matchID;
        public bool publicMatch;
        public bool inMatch;
        public bool matchFull;
        public List<Player> players = new List<Player> ();

        public Match (string matchID, Player player, bool publicMatch) {
            matchFull = false;
            inMatch = false;
            this.matchID = matchID;
            this.publicMatch = publicMatch;
            players.Add (player);
        }

        public Match () { }
    }

    public class MatchMaker : NetworkBehaviour {

        public static MatchMaker instance;

        public SyncList<Match> matches = new SyncList<Match> ();
        public SyncList<String> matchIDs = new SyncList<String> ();

        [SerializeField] int maxMatchPlayers = 12;

        private void Awake () {
            instance = this;
        }

        public bool HostGame (string matchID, Player player, bool publicMatch, out int playerIndex) {
            playerIndex = -1;

            if (!matchIDs.Contains (matchID)) {
                matchIDs.Add (matchID);
                Match match = new Match (matchID, player, publicMatch);
                matches.Add (match);
                Debug.Log ($"Match generated");
                //player.currentMatch = match;
                playerIndex = 1;
                return true;
            } else {
                Debug.Log ($"Match ID already exists");
                return false;
            }
        }

        public bool JoinGame (string matchID, Player player, out int playerIndex) {
            playerIndex = -1;

            if (matchIDs.Contains (matchID)) {

                for (int i = 0; i < matches.Count; i++) {
                    if (matches[i].matchID == matchID) {
                        if (!matches[i].inMatch && !matches[i].matchFull) {
                            matches[i].players.Add (player);
                            //player.currentMatch = matches[i];
                            playerIndex = matches[i].players.Count;

                            matches[i].players[0].PlayerCountUpdated (matches[i].players.Count);

                            if (matches[i].players.Count == maxMatchPlayers) {
                                matches[i].matchFull = true;
                            }

                            break;
                        } else {
                            return false;
                        }
                    }
                }

                Debug.Log ($"Match joined");
                return true;
            } else {
                Debug.Log ($"Match ID does not exist");
                return false;
            }
        }

        public bool SearchGame (Player player, out int playerIndex, out string matchID) {
            playerIndex = -1;
            matchID = "";

            for (int i = 0; i < matches.Count; i++) {
                Debug.Log ($"Checking match {matches[i].matchID} | inMatch {matches[i].inMatch} | matchFull {matches[i].matchFull} | publicMatch {matches[i].publicMatch}");
                if (!matches[i].inMatch && !matches[i].matchFull && matches[i].publicMatch) {
                    if (JoinGame (matches[i].matchID, player, out playerIndex)) {
                        matchID = matches[i].matchID;
                        return true;
                    }
                }
            }

            return false;
        }

        public void BeginGame (string matchID) {
            for (int i = 0; i < matches.Count; i++) {
                if (matches[i].matchID == matchID) {
                    matches[i].inMatch = true;
                    foreach (var player in matches[i].players) {
                        player.StartGame ();
                    }
                    break;
                }
            }
        }

        public static string GetRandomMatchID () {
            string id = string.Empty;
            for (int i = 0; i < 5; i++) {
                int random = UnityEngine.Random.Range (0, 36);
                if (random < 26) {
                    id += (char) (random + 65);
                } else {
                    id += (random - 26).ToString ();
                }
            }
            Debug.Log ($"Random Match ID: {id}");
            return id;
        }

        public void PlayerDisconnected (Player player, string matchID) {
            for (int i = 0; i < matches.Count; i++) {
                if (matches[i].matchID == matchID) {
                    int playerIndex = matches[i].players.IndexOf (player);
                    if (matches[i].players.Count > playerIndex) matches[i].players.RemoveAt (playerIndex);
                    Debug.Log ($"Player disconnected from match {matchID} | {matches[i].players.Count} players remaining");

                    if (matches[i].players.Count == 0) {
                        Debug.Log ($"No more players in Match. Terminating {matchID}");
                        matches.RemoveAt (i);
                        matchIDs.Remove (matchID);
                    } else {
                        matches[i].players[0].PlayerCountUpdated (matches[i].players.Count);
                    }
                    break;
                }
            }
        }

    }

    public static class MatchExtensions {
        public static Guid ToGuid (this string id) {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider ();
            byte[] inputBytes = Encoding.Default.GetBytes (id);
            byte[] hashBytes = provider.ComputeHash (inputBytes);

            return new Guid (hashBytes);
        }
    }

}