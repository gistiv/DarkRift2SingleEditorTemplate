using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.Debugging.Console.Commands;

namespace Utility.Debugging.Console.Input
{
	
	public class DebugConsoleInput : MonoBehaviour
	{
		public bool DisplayConsoleInput = false;

		private string input;

		private int inputHeight = 25;

		private List<object> commandList;

		private List<string> inputHistory;

		private int currentHistoryIndex = -1;

		// Start is called before the first frame update
		void Awake()
	    {
			// add help command, needs to be done here as the command list that needs to be itterated is private
			commandList = new List<object>
			{
				new DebugCommand<string>("help", "Prints all available commands", "help", () =>
				{
					string logString = string.Empty;

					foreach (DebugCommandBase command in commandList)
					{
						logString += $"{command.FormatExample} - {command.Description}{Environment.NewLine}";
					}

					return logString;
				})
			};

			commandList.AddRange(DebugCommandsCommon.Commands);

			inputHistory = new List<string>();

		}

		public void RegisterServerCommands()
        {
			commandList.AddRange(DebugCommandsServer.Commands);
		}

		public void RegisterClientCommands()
		{
			commandList.AddRange(DebugCommandsClient.Commands);
		}

		private void HandleInput()
        {
			inputHistory.Add(input);
			string[] parameters = input.Split(' '); 

            foreach (DebugCommandBase command in commandList)
            {
                if (parameters[0] == command.Id)
                {
					if (command as DebugCommand<string> != null)
                    {
						(command as DebugCommand<string>).Invoke(input);
						return;
                    }
					else if (command as DebugCommand<string> != null)
					{
						(command as DebugCommand<string, string>).Invoke(input, parameters[1]);
						return;
					}
					else if (command as DebugCommand<int, Vector3, string> != null)
					{
						(command as DebugCommand<int, Vector3, string>).Invoke(input, int.Parse(parameters[1]), new Vector3(float.Parse(parameters[2]), float.Parse(parameters[3]), float.Parse(parameters[4])));
						return;
					}
				}
            }

			Debug.LogWarning($"Command \"{input}\" could not be processed");
        }

        private void OnGUI()
        {
            if (DisplayConsoleInput)
            {
				Event e = Event.current;
				if (e.keyCode == KeyCode.Return && e.type == EventType.KeyDown && !string.IsNullOrWhiteSpace(input))
                {
					HandleInput();
					ResetInputField();
				}
				else if (e.keyCode == KeyCode.DownArrow && e.type == EventType.KeyDown && inputHistory.Any())
				{
					currentHistoryIndex++;

					if (currentHistoryIndex > inputHistory.Count - 1)
					{
						currentHistoryIndex = 0;
					}

					input = inputHistory[currentHistoryIndex];
				}
				else if (e.keyCode == KeyCode.UpArrow && e.type == EventType.KeyDown && inputHistory.Any())
                {
					currentHistoryIndex--;

					if (currentHistoryIndex < 0)
					{
						currentHistoryIndex = inputHistory.Count - 1;
					}

					input = inputHistory[currentHistoryIndex];
				}
                else if (e.keyCode == KeyCode.C && e.type == EventType.KeyDown && e.control)
                {
                    // close console when inside input
                    DebugUtility.Instance.ToogleFromInput = true;
                }

                // to make the input field darker
                GUI.Box(new Rect(0, Screen.height - inputHeight, Screen.width, inputHeight), "", GUI.skin.box);
				GUI.Box(new Rect(0, Screen.height - inputHeight, Screen.width, inputHeight), "",GUI.skin.box);

				input = GUI.TextField(new Rect(0, Screen.height - inputHeight, Screen.width, inputHeight), input, GUI.skin.box);
            }
            else
            {
				ResetInputField();
			}
		}

		private void ResetInputField()
        {
			input = string.Empty;
			currentHistoryIndex = -1;
		}
    }
	
}
