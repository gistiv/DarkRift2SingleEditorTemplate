using UnityEngine;
using Utility.Debugging.Console.Input;
using Utility.Debugging.Console.Log;

namespace Utility.Debugging
{
	[RequireComponent(typeof(DebugConsole))]
	[RequireComponent(typeof(DebugConsoleInput))]
	public class DebugUtility : MonoBehaviour
	{
		public static DebugUtility Instance;

		public bool DebugConsoleOverlayActive = false;

		public bool ToogleConsoleFromInput = false;

		private DebugConsole console;
		private DebugConsoleInput input;
		private DebugGraphs graphs;


		void Awake()
	    {
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(this);

			console = GetComponent<DebugConsole>();
			input = GetComponent<DebugConsoleInput>();
			graphs = GetComponent<DebugGraphs>();

			graphs.enabled = false;
	    }

	    // Update is called once per frame
	    void Update()
	    {
            if (ToogleConsoleFromInput || (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.C))
            {
				DebugConsoleOverlayActive = !DebugConsoleOverlayActive;
				ToogleConsoleFromInput = false;

				console.DisplayConsole = DebugConsoleOverlayActive;
				input.DisplayConsoleInput = DebugConsoleOverlayActive;

				if (DebugConsoleOverlayActive)
				{
					Cursor.lockState = CursorLockMode.None;
				}
				else
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
	    }

		public void ToggleDebugGraphs()
        {
			graphs.enabled = !graphs.enabled;
		}

		public void RegisterAsServer()
        {
			input.RegisterServerCommands();
        }

		public void RegisterAsClient()
		{
			input.RegisterClientCommands();

			if(graphs.enabled == false)
            {
				graphs.enabled = true;
				graphs.RegisterAsClient();
				graphs.enabled = false;
			}
            else
            {
				graphs.RegisterAsClient();
            }
		}
	}
	
}
