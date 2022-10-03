﻿using Common;
using Common.NetworkingData;
using Common.Player;
using DarkRift;
using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{

	[RequireComponent(typeof(PlayerLogic))]
	public class PlayerServer : MonoBehaviour
	{
		public PlayerLogic PlayerLogic { get; private set; }
		public uint InputTick { get; private set; }
		public IClient Client { get; private set; }
		public PlayerStateData CurrentPlayerStateData => currentPlayerStateData;

		private ClientConnection clientConnection;
		private ServerInstance serverInstance;

		private PlayerStateData currentPlayerStateData;

		private Buffer<PlayerInputData> inputBuffer = new Buffer<PlayerInputData>(1, 2);

		private PlayerInputData[] inputs;

		void Awake()
		{
			PlayerLogic = GetComponent<PlayerLogic>();
		}

		public void Initialize(Vector3 position, ClientConnection clientConnection)
		{
			this.clientConnection = clientConnection;
			serverInstance = clientConnection.ServerInstance;
			Client = clientConnection.Client;
			this.clientConnection.Player = this;

			currentPlayerStateData = new PlayerStateData(Client.ID, 0, position, 0f, 0f);
			InputTick = serverInstance.ServerTick;

            PlayerSpawnData[] playerSpawnData = serverInstance.GetSpawnDataForAllPlayers();
			using (Message m = Message.Create((ushort)NetworkingTags.GameStartDataResponse, new GameStartData(playerSpawnData, serverInstance.ServerTick)))
			{
				Client.SendMessage(m, SendMode.Reliable);
			}
		}

		public void RecieveInput(PlayerInputData input)
		{
			inputBuffer.Add(input);
		}

		public PlayerStateData PlayerUpdate()
		{
			inputs = inputBuffer.Get();

			if (inputs.Length > 0)
			{
				PlayerInputData input = inputs[0];
				InputTick++;

				for (int i = 1; i < inputs.Length; i++)
				{
					InputTick++;
					for (int j = 0; j < input.Keyinputs.Length; j++)
					{
						input.Keyinputs[j] = input.Keyinputs[j] || inputs[i].Keyinputs[j];
					}

					input.Yaw = inputs[i].Yaw;
					input.Pitch = inputs[i].Pitch;
				}

				currentPlayerStateData = PlayerLogic.GetNextFrameData(input, currentPlayerStateData);
			}

			transform.localPosition = currentPlayerStateData.Position;
			transform.localRotation = Quaternion.Euler(0.0f, currentPlayerStateData.Yaw, 0.0f);
			return currentPlayerStateData;
		}

		public PlayerSpawnData GetPlayerSpawnData()
		{
			return new PlayerSpawnData(Client.ID, clientConnection.Name, transform.localPosition);
		}
	}
	
}
