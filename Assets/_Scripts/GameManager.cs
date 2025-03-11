using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

namespace TotalWarfare
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager instance;
        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
        
        #region Private Methods

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Not able to load because you are not Master Client");
                return;
            }
            PhotonNetwork.LoadLevel(1);
            PhotonNetwork.Instantiate("TestPlayer", Vector3.zero, Quaternion.identity, 0);
        }
        
        #endregion

        private void OnGUI()
        {
            GUI.TextField(new Rect(0, 0, 225, 25), "Room Name: " + PhotonNetwork.CurrentRoom.Name);
            GUI.TextField(new Rect(0, 30, 200, 25), "Player Count: " + PhotonNetwork.PlayerList.Length);
        }
    }
}