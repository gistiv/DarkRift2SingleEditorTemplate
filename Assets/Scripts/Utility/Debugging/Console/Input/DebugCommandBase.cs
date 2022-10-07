using System;
using UnityEngine;

namespace Utility.Debugging.Console.Input
{
	
	public class DebugCommandBase 
	{
        public string Id { get; set; }
		public string Description { get; set; }
		public string FormatExample { get; set; }

		public DebugCommandBase(string commandId, string commandDescription, string commandFormat)
		{
			Id = commandId.ToLower();
			Description = commandDescription;
			FormatExample = commandFormat;
		}

		public virtual void Invoke(string fullCommand, string logOutput)
        {
			Debug.Log($"{fullCommand}{Environment.NewLine}{logOutput}"); ;
        }
	}
	
}
