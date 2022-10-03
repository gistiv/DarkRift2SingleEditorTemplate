using Common;
using Common.NetworkingData;
using DarkRift;
using DarkRift.Client;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Prefabs")]
        public GameObject PlayerPrefab;

        public uint ClientTick { get; private set; }
        public uint LastReceivedServerTick { get; private set; }

        private Buffer<GameUpdateData> gameUpdateDataBuffer = new Buffer<GameUpdateData>(1, 1);

        private Dictionary<ushort, PlayerClient> players = new Dictionary<ushort, PlayerClient>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            ConnectionManager.Instance.Client.MessageReceived += OnMessage;
            using (Message message = Message.CreateEmpty((ushort)NetworkingTags.GameJoinRequest))
            {
                ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
            }
        }

        void FixedUpdate()
        {
            ClientTick++;
            GameUpdateData[] receivedGameUpdateData = gameUpdateDataBuffer.Get();
            foreach (GameUpdateData data in receivedGameUpdateData)
            {
                UpdateClientGameState(data);
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                switch ((NetworkingTags)message.Tag)
                {
                    case NetworkingTags.GameStartDataResponse:
                        OnGameJoinAccept(message.Deserialize<GameStartData>());
                        break;
                    case NetworkingTags.GameUpdate:
                        OnGameUpdate(message.Deserialize<GameUpdateData>());
                        break;
                }
            }
        }

        void UpdateClientGameState(GameUpdateData gameUpdateData)
        {
            LastReceivedServerTick = gameUpdateData.Frame;
            foreach (PlayerSpawnData data in gameUpdateData.SpawnData)
            {
                if (data.Id != ConnectionManager.Instance.PlayerId)
                {
                    SpawnPlayer(data);
                }
            }

            foreach (PlayerDespawnData data in gameUpdateData.DespawnData)
            {
                if (players.ContainsKey(data.Id))
                {
                    Destroy(players[data.Id].gameObject);
                    players.Remove(data.Id);
                }
            }

            foreach (PlayerStateData data in gameUpdateData.UpdateData)
            {
                PlayerClient p;
                if (players.TryGetValue(data.Id, out p))
                {
                    p.OnServerDataUpdate(data);
                }
            }
        }

        void OnGameJoinAccept(GameStartData gameStartData)
        {
            LastReceivedServerTick = gameStartData.OnJoinServerTick;
            ClientTick = gameStartData.OnJoinServerTick;
            foreach (PlayerSpawnData playerSpawnData in gameStartData.Players)
            {
                SpawnPlayer(playerSpawnData);
            }
        }

        void OnGameUpdate(GameUpdateData gameUpdateData)
        {
            gameUpdateDataBuffer.Add(gameUpdateData);
        }

        void SpawnPlayer(PlayerSpawnData playerSpawnData)
        {
            GameObject go = Instantiate(PlayerPrefab);
            // PlayerInterpolation gets added through the RequireComponentent of the PlayerClient
            PlayerClient player = go.AddComponent<PlayerClient>();

            player.Initialize(playerSpawnData.Id, playerSpawnData.Name);
            players.Add(playerSpawnData.Id, player);
        }

    }
	
}
