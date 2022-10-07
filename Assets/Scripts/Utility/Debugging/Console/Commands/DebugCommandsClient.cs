using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Debugging.Console.Input;

namespace Utility.Debugging.Console.Commands
{
	
	public static class DebugCommandsClient
	{
		public static List<object> Commands;

		static DebugCommandsClient()
		{
			Commands = new List<object>
			{
				new DebugCommand<string>("test", "Test command client", "test", () =>
				{
					return "Hello from the client";
				})
			};
		}
	}
	
}
