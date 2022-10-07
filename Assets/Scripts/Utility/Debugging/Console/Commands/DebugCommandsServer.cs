using Server;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Debugging.Console.Input;

namespace Utility.Debugging.Console.Commands
{
	
	public static class DebugCommandsServer
	{
		public static List<object> Commands;

		static DebugCommandsServer()
		{
			Commands = new List<object>
			{
				new DebugCommand<string>("test", "Test command server", "test", () =>
				{
					return "Hello from the server";
				}),

				new DebugCommand<string>("list-players", "Prints a list of all connected player", "players", () =>
				{
					string logString = string.Empty;

					foreach (ClientConnection player in ServerManager.Instance.Players.Values)
					{
						logString += $"{player.Client.ID} - {player.Name}{Environment.NewLine}";
					}

					return logString;
				}),

				new DebugCommand<int, Vector3, string>("move-player", "Instantly moves a player with the given id(uint) to the given coordinate", "move-player <id> <x> <y> <z>", (id, pos) =>
				{
					ServerManager.Instance.ServerInstance.DisplacePlayer(id, pos);
					return string.Empty;
				})
			};
		}
	}
	
}
