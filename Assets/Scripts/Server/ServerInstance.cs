using Common.NetworkingData;
using Common.Player;
using DarkRift;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

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

        private Dictionary<ushort, uint> shootCooldowns = new Dictionary<ushort, uint>();

        private List<PlayerServer> serverPlayers = new List<PlayerServer>();

        private List<PlayerStateData> playerStateData = new List<PlayerStateData>();

        private List<PlayerSpawnData> playerSpawnData = new List<PlayerSpawnData>();

        private List<PlayerDespawnData> playerDespawnData = new List<PlayerDespawnData>();

        private List<PlayerFireWeaponData> playerFireWeaponData = new List<PlayerFireWeaponData>();

        private List<BulletHitData> bulletHitData = new List<BulletHitData>();

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

            foreach (PlayerServer player in serverPlayers)
            {
                player.PlayerPreUpdate();
            }

            for (int i = 0; i < serverPlayers.Count; i++)
            {
                PlayerServer player = serverPlayers[i];
                playerStateData[i] = player.PlayerUpdate();
            }

            // Send update message to all players.
            PlayerStateData[] playerStateDataArray = playerStateData.ToArray();
            PlayerSpawnData[] playerSpawnDataArray = playerSpawnData.ToArray();
            PlayerDespawnData[] playerDespawnDataArray = playerDespawnData.ToArray();
            PlayerFireWeaponData[] playerFireWeaponDataArray = playerFireWeaponData.ToArray();
            BulletHitData[] bulletHitDataArray = bulletHitData.ToArray();

            foreach (PlayerServer p in serverPlayers)
            {
                using (Message m = Message.Create((ushort)NetworkingTags.GameUpdate, new GameUpdateData(p.InputTick, playerStateDataArray, playerSpawnDataArray, playerDespawnDataArray, playerFireWeaponDataArray, bulletHitDataArray)))
                {
                    p.Client.SendMessage(m, SendMode.Reliable);
                }
            }

            playerSpawnData.Clear();
            playerDespawnData.Clear();
            playerFireWeaponData.Clear();
            bulletHitData.Clear();
        }

        public void PerformShootRayCast(uint frame, PlayerServer shooter)
        {
            ushort clientId = shooter.Client.ID;

            if (shootCooldowns[clientId] > frame)
            {
                Debug.LogWarning($"Player {clientId}-\"{shooter.name}\" tried to shoot without permission");
                return;
            }

            int dif = (int)(ServerTick - 1 - frame);

            uint cooldownFrame = WeaponsCalculationHelper.CaclulateShootCooldownFrame(frame);
            shootCooldowns[clientId] = cooldownFrame;

            playerFireWeaponData.Add(new PlayerFireWeaponData(clientId));

            // Get the position of the ray
            Vector3 startPosition;
            Vector3 direction;

            if (shooter.PlayerStateDataHistory.Count > dif)
            {
                startPosition = new Vector3(shooter.PlayerStateDataHistory[dif].Position.x, shooter.PlayerStateDataHistory[dif].Position.y + 1.5f, shooter.PlayerStateDataHistory[dif].Position.z);
                direction = Quaternion.Euler(shooter.PlayerStateDataHistory[dif].Pitch, shooter.PlayerStateDataHistory[dif].Yaw, 0.0f) * Vector3.forward;
            }
            else
            {
                startPosition = new Vector3(shooter.CurrentPlayerStateData.Position.x, shooter.CurrentPlayerStateData.Position.y + 1.5f, shooter.CurrentPlayerStateData.Position.z);
                direction = Quaternion.Euler(shooter.CurrentPlayerStateData.Pitch, shooter.CurrentPlayerStateData.Yaw, 0.0f) * Vector3.forward;
            }

            startPosition += direction * 0.5f;

            //set all players back in time
            foreach (PlayerServer player in serverPlayers)
            {
                if (player.PlayerStateDataHistory.Count > dif)
                {
                    player.PlayerLogic.CharacterController.enabled = false;
                    player.transform.localPosition = player.PlayerStateDataHistory[dif].Position;
                }
            }

            RaycastHit hit;
            Debug.DrawRay(startPosition, direction * 200, Color.green, 5f);
            if (physicsScene.Raycast(startPosition, direction, out hit, 200f))
            {
                if (hit.transform.CompareTag(nameof(Tags.Player)))
                {
                    bulletHitData.Add(new BulletHitData(hit.transform.GetComponent<PlayerServer>().Client.ID, hit.point, hit.normal));
                }
            }

            // Set all players back.
            foreach (PlayerServer player in serverPlayers)
            {
                player.transform.localPosition = player.CurrentPlayerStateData.Position;
                player.PlayerLogic.CharacterController.enabled = true;
            }
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
            playerStateData.RemoveAt(playerStateData.FindIndex(ps => ps.Id == clientConnection.Client.ID));
            shootCooldowns.Remove(clientConnection.Client.ID);
            clientConnection.ServerInstance = null;
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
            GameObject go = Instantiate(Resources.Load<GameObject>(@"Prefabs\Gameplay\Player\Player"), transform);
            
            PlayerServer player = go.AddComponent<PlayerServer>();

            serverPlayers.Add(player);
            playerStateData.Add(default);
            shootCooldowns.Add(clientConnection.Client.ID, ServerTick);

            player.Initialize(Vector3.zero, clientConnection);

            ClientConnections.Add(clientConnection);
            clientConnection.ServerInstance = this;

            playerSpawnData.Add(player.GetPlayerSpawnData());

        }
    }
	
}
