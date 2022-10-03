using Common;
using Common.NetworkingData;
using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server
{
	
	public class ServerInstance : MonoBehaviour
	{
        [Header("Prefabs")]
        [SerializeField]
        public GameObject PlayerPrefab;

        [Header("Public Fields")]
        public string Name;

        public byte MaxSlots;

        public uint ServerTick;

        public List<ClientConnection> ClientConnections = new List<ClientConnection>();

        private Scene scene;

        private PhysicsScene physicsScene;

        private List<PlayerServer> serverPlayers = new List<PlayerServer>();

        private List<PlayerStateData> playerStateData = new List<PlayerStateData>(4);

        private List<PlayerSpawnData> playerSpawnData = new List<PlayerSpawnData>(4);

        private List<PlayerDespawnData> playerDespawnData = new List<PlayerDespawnData>(4);

        void Start()
        {
            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            scene = SceneManager.CreateScene("Server" + name, csp);
            physicsScene = scene.GetPhysicsScene();

            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }

        void FixedUpdate()
        {
            ServerTick++;

            for (int i = 0; i < serverPlayers.Count; i++)
            {
                PlayerServer player = serverPlayers[i];
                playerStateData[i] = player.PlayerUpdate();
            }

            // Send update message to all players.
            PlayerStateData[] playerStateDataArray = playerStateData.ToArray();
            PlayerSpawnData[] playerSpawnDataArray = playerSpawnData.ToArray();
            PlayerDespawnData[] playerDespawnDataArray = playerDespawnData.ToArray();
            foreach (PlayerServer p in serverPlayers)
            {
                using (Message m = Message.Create((ushort)NetworkingTags.GameUpdate, new GameUpdateData(p.InputTick, playerStateDataArray, playerSpawnDataArray, playerDespawnDataArray)))
                {
                    p.Client.SendMessage(m, SendMode.Reliable);
                }
            }

            playerSpawnData.Clear();
            playerDespawnData.Clear();
        }

        public void AddPlayer(ClientConnection clientConnection)
        {
            ClientConnections.Add(clientConnection);
            clientConnection.ServerInstance = this;
        }


        public void RemovePlayer(ClientConnection clientConnection)
        {
            Destroy(clientConnection.Player.gameObject);

            playerDespawnData.Add(new PlayerDespawnData(clientConnection.Client.ID));
            ClientConnections.Remove(clientConnection);
            serverPlayers.Remove(clientConnection.Player);
            clientConnection.ServerInstance = null;
        }

        public void Close()
        {
            foreach (ClientConnection p in ClientConnections)
            {
                RemovePlayer(p);
            }

            Destroy(gameObject);
        }

        public PlayerSpawnData[] GetSpawnDataForAllPlayers()
        {
            PlayerSpawnData[] playerSpawnData = new PlayerSpawnData[serverPlayers.Count];
            for (int i = 0; i < serverPlayers.Count; i++)
            {
                PlayerServer p = serverPlayers[i];
                playerSpawnData[i] = p.GetPlayerSpawnData();
            }

            return playerSpawnData;
        }

        public void JoinPlayerToGame(ClientConnection clientConnection)
        {
            GameObject go = Instantiate(PlayerPrefab, transform);           
            PlayerServer player = go.AddComponent<PlayerServer>();
            serverPlayers.Add(player);
            playerStateData.Add(default);
            player.Initialize(Vector3.zero, clientConnection);

            playerSpawnData.Add(player.GetPlayerSpawnData());
        }
    }
	
}
