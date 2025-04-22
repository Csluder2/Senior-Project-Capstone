using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Buffers;
using TMPro;

namespace Csluder2.FusionWork
{

    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks //IBeforeUpdate

    {
        [HideInInspector] public NetworkRunner runner;
        public static FusionConnection instance;
        [SerializeField] NetworkObject playerPrefab;
        private bool isHost = false;

        public GameObject Hostwaiting;

        public GameObject Hostjoining;
        private bool resetInput;

        private string _playerName = "";



        public GameObject Scrollview;

        [Header("Session List")]
        private List<SessionInfo> _sessions = new List<SessionInfo>();
        public Button refreshButton;
        public Transform sessionListContent;
        public GameObject sessionEntryPrefab;
        private PlayerInputData accumulatedInput;
        public Boolean firstLobby = false;
        private void Awake()
        {
            if (instance == null) { instance = this; }


        }

        public struct PlayerInputData : INetworkInput
        {
            public NetworkButtons Buttons;
            public Vector2 Direction;
        }

        public enum PlayerButtons
        {
            MoveLeft = 0,
            MoveRight = 1,
            Jump = 2,
            Attack = 3,
            Block = 4,
            Crouch = 5
        }




        public void ConnectToLobby(string playerName)
        {
            _playerName = playerName;

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
            runner.JoinSessionLobby(SessionLobby.Shared);

        }
        public async void ConnectToSession(string SessionName)
        {

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }


            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = SessionName,
                PlayerCount = 2

            });

        }
        public async void CreateSession()
        {
            Scrollview.SetActive(false);
            Hostwaiting.SetActive(true);
            isHost = true;
            int randomInt = UnityEngine.Random.Range(1000, 9999);
            string randomSessionName = _playerName + "s  Room-" + randomInt.ToString();
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }


            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = randomSessionName,
                PlayerCount = 2,
            });
        }
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");

            //NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
            //runner.SetPlayerObject(runner.LocalPlayer, playerObject);

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {


        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var playerInput = new PlayerInputData();
            playerInput.Buttons.Set((int)PlayerButtons.MoveLeft, Input.GetKey(KeyCode.A));
            playerInput.Buttons.Set((int)PlayerButtons.MoveRight, Input.GetKey(KeyCode.D));
            playerInput.Buttons.Set((int)PlayerButtons.Jump, Input.GetKey(KeyCode.W));
            playerInput.Buttons.Set((int)PlayerButtons.Attack, Input.GetKey(KeyCode.P));
            playerInput.Buttons.Set((int)PlayerButtons.Block, Input.GetKey(KeyCode.O));
            playerInput.Buttons.Set((int)PlayerButtons.Crouch, Input.GetKey(KeyCode.S));
            input.Set(playerInput);
            //accumulatedInput.Direction.Normalize();
            //input.Set(accumulatedInput);
            //resetInput = true;
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

            Debug.Log("Player has joined!");


            if (runner.SessionInfo.PlayerCount == 2)
            {
                if (runner.IsSceneAuthority)
                {
                    Hostwaiting.SetActive(false);
                    Hostjoining.SetActive(true);
                }
                Debug.Log("Two Players are in the room, loading FightingStage");
                runner.LoadScene(SceneRef.FromIndex(8), LoadSceneMode.Single);

            }

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            runner.LoadScene(SceneRef.FromIndex(7), LoadSceneMode.Single);
            _playerName = null;
            isHost = false;
            runner.Shutdown();
        }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (runner.IsSceneAuthority)
            {
                Debug.Log("Scene Authority confirmed.");
            }

            // Every client should spawn their own character
            if (SceneManager.GetActiveScene().name == "OnlineFightingStage")
                StartCoroutine(SpawnPlayer(runner));

            if (SceneManager.GetActiveScene().name == "MainMenu")
                runner.Shutdown();


        }

        private IEnumerator SpawnPlayer(NetworkRunner runner)
        {
            yield return new WaitForSeconds(0.1f); // Let the scene settle a bit
            Debug.Log("This clients player ID is" + runner.LocalPlayer.PlayerId);
            Vector3 spawnPos = runner.LocalPlayer.PlayerId == 1 ? new Vector3(-3, 2, 0) : new Vector3(3, 2, 0);
            Quaternion spawnRot = runner.LocalPlayer.PlayerId == 1 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);

            runner.Spawn(playerPrefab, spawnPos, spawnRot, runner.LocalPlayer);

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("FightingStage Being loaded");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            _sessions.Clear();
            _sessions = sessionList;
            if (firstLobby == false)
            {
                RefreshSessionListUI();
                firstLobby = true;
            }
        }
        public void RefreshSessionListUI()
        {
            foreach (Transform child in sessionListContent)
            {
                Destroy(child.gameObject);
            }

            foreach (SessionInfo session in _sessions)
            {
                if (session.IsVisible)
                {
                    GameObject entry = GameObject.Instantiate(sessionEntryPrefab, sessionListContent);
                    SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                    script.sessionName.text = session.Name;
                    script.playerCount.text = session.PlayerCount + "/" + session.MaxPlayers;

                    if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                    {
                        script.joinButton.interactable = false;
                    }
                    else
                    {
                        script.joinButton.interactable = true;
                    }

                }
            }
        }



        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {


        }


    }
}



