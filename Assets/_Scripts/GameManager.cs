using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

namespace TotalWarfare
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager instance;
        
        [SerializeField] private GameObject waitinForPlayers;
        public bool isGameStarted = false;
        [SerializeField] GameObject[] playerPrefabs;
        [SerializeField]
        private Transform[] spawns;
        private int player1;

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
        
            PhotonNetwork.Instantiate("Camera", new Vector3(0, 40, 0), Quaternion.Euler(75, 180, 0));
            for (int i = 0; i < this.playerPrefabs.Length; i++)
            {
                PhotonNetwork.Instantiate(this.playerPrefabs[i].name, spawns[PhotonNetwork.CurrentRoom.PlayerCount-1].position, Quaternion.identity);
            }
            
            Camera.main.transform.parent.eulerAngles += Vector3.up * 180;
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
        
        #region Private Methods

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {

            if (!PhotonNetwork.IsConnectedAndReady)
                return;
            PhotonNetwork.Instantiate("Camera", new Vector3(0, 40, 0), Quaternion.Euler(75, 0, 0));
            for (int i = 0; i < this.playerPrefabs.Length; i++)
            {
                GameObject go = PhotonNetwork.Instantiate(this.playerPrefabs[i].name, spawns[PhotonNetwork.CurrentRoom.PlayerCount-1].position, Quaternion.identity);
                if (i == 0)
                {
                    player1 = go.GetComponent<PhotonView>().ViewID;
                }
            }
        }

        private void Update()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                isGameStarted = true;
                waitinForPlayers.SetActive(false);
            }
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

        // IEnumerator GameStart()
        // {
        //     for (int i = 0; i < this.playerPrefabs.Length; i++)
        //     {
        //         PhotonNetwork.Instantiate(this.playerPrefabs[i].name, spawns[PhotonNetwork.CurrentRoom.PlayerCount-1].position, Quaternion.identity);
        //     }
        // }
        
        #endregion

#if DEVELOPMENT_BUILD || UNITY_EDITOR
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