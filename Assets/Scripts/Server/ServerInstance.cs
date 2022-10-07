using Common.NetworkingData;
using DarkRift;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server
{

    public class ServerInstance : MonoBehaviour
	{
        [Header("Public Fields")]
        public byte MaxSlots = 25;

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
            scene = SceneManager.CreateScene(gameObject.name, csp);
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

        // this method is just for demonstration purpose of the debug console
        public void DisplacePlayer(int playerId, Vector3 newPosition)
        {           
            ClientConnection connection = ClientConnections.SingleOrDefault(x => x.Client.ID == playerId);

            if(connection != null)
            {
                connection.Player.SpawnPlayerAtPosition(newPosition);
            }
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
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs\\Gameplay\\Player\\Player"), transform);
            
            PlayerServer player = go.AddComponent<PlayerServer>();
            serverPlayers.Add(player);
            playerStateData.Add(default);
            player.Initialize(Vector3.zero, clientConnection);

            ClientConnections.Add(clientConnection);
            clientConnection.ServerInstance = this;

            playerSpawnData.Add(player.GetPlayerSpawnData());
        }
    }
	
}
