using System.Collections.Generic;
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
				new DebugCommand<string>("debug-graphs", "Toggles the debug graphs", "debug-graphs", () =>
				{
					DebugUtility.Instance.ToggleDebugGraphs();
					return string.Empty;
				})
			};
		}
	}
	
}
