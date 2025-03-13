using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace TotalWarfare
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject _playerPrefab;

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

        public override void OnJoinedRoom()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
                return;
            
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0, 5, 0), Quaternion.identity, 0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
        
        #region Private Methods

        private void Start()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
                return;
            
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0, 5, 0), Quaternion.identity, 0);
        }
        
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Not able to load because you are not Master Client");
                return;
            }
            //PhotonNetwork.LoadLevel(1);
            // PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }
        
        #endregion

#if DEVELOPMENT_BUILD
        private void OnGUI()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                GUI.TextField(new Rect(0, 0, 225, 25), "Room Name: " + PhotonNetwork.CurrentRoom.Name);
                GUI.TextField(new Rect(0, 30, 200, 25), "Player Count: " + PhotonNetwork.PlayerList.Length);
                GUI.TextField(new Rect(0, 50, 200, 25), "IsConnectedToNetwork: " + PhotonNetwork.IsConnected);
            }
        }
#endif
    }
}