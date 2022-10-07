using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Debugging.Console.Input;

namespace Utility.Debugging.Console.Commands
{
	
	public static class DebugCommandsCommon
	{
		public static List<object> Commands;

		static DebugCommandsCommon()
        {
			Commands = new List<object>
			{
				new DebugCommand<string>("common-test", "Test command common", "common-test", () =>
				{
					return "Hello from common";
				}),

				new DebugCommand<string, string>("string-input-test", "Logs the given input", "string-input-test <input>", (value) =>
				{
					return value;
				})
			};
		}
	}
	
}
